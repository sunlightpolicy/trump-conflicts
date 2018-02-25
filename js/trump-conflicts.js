


d3.json("data/conflicts.json", function (data) {
    data.forEach(function (d) {
        d.sourceType = "Office of Government Ethics";
        if ((typeof(d.sources[0]) != "undefined") && (d.sources[0].name != "Office of Government Ethics"))
            d.sourceType = "Media";

        d.source = "N/A";
        if (typeof (d.sources[0]) != "undefined") {
            d.source = d.sources[0].name;

            if ((typeof (d.sources[1]) != "undefined") && (d.sources[1].name != d.source))
                d.source = "Multiple Sources";
        }

        if (d.conflictingEntity == "")
            d.conflictingEntity = "N/A";

        d.description = d.category + " - " + d.description;

        d.links = getLinks(d);
    });
    var facts = crossfilter(data);

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);


    var pieRadius = 70;
    var pieWidthAndHeight = 150;

    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue

    var categoryDim = facts.dimension(dc.pluck('category'));
    dc.pieChart("#dc-chart-category")
        .dimension(categoryDim)
        .group(categoryDim.group().reduceCount())
        .width(pieWidthAndHeight)
        .height(pieWidthAndHeight)
        .radius(pieRadius)
        .ordinalColors(pieColors);

    var familyMemberDim = facts.dimension(dc.pluck('familyMember'));
    dc.pieChart("#dc-chart-familyMember")
        .dimension(familyMemberDim)
        .group(familyMemberDim.group().reduceCount())
        .width(pieWidthAndHeight)
        .height(pieWidthAndHeight)
        .radius(pieRadius)
        .ordinalColors(pieColors);

    // Not used
    var sourceTypeDim = facts.dimension(dc.pluck('sourceType'));
    dc.pieChart("#dc-chart-sourceType")
        .dimension(sourceTypeDim)
        .group(sourceTypeDim.group().reduceCount())
        .width(pieWidthAndHeight)
        .height(pieWidthAndHeight)
        .radius(pieRadius)
        .ordinalColors(pieColors)


    //new RowChart(facts, "familyMember", 200, 6);
    new RowChart(facts, "conflictingEntity", 240, 400);
    new RowChart(facts, "source", 160, 40);
    //new RowChart(facts, "category", 300, 3);
    //new RowChart(facts, "sourceType", 300, 2);
    
    dataTable = dc.dataTable("#dc-chart-table");

    var tableDim = facts.dimension(function(d) { return +d.Id; });

    dataTable
        .dimension(tableDim)
        .group(function (d) {
            return d.familyMember;
        })
        //.group(function (d) { })
        //.showGroups(false)
        .size(50)
        //.size(xf.size()) //display all data
        .columns([
            function (d) { return d.description; },
            function (d) { return d.notes; },
            function(d) { return d.conflictingEntity; },
            function(d) { return d.links; }
        ])
        .sortBy(function(d){ return d.conflictingEntity; })
        .order(d3.ascending)
        .renderlet(function (table) {
            table.selectAll(".dc-table-group").classed("info", true);
        });
   
    dc.renderAll();
});


function getLinks(d) {
    var links = "";
    for (i = 0; i < d.sources.length; i++) {
        if (links != "")
            links = links + '<br>';
        links = links + '<a href="' + d.sources[i].link + '" target="_blank">' + d.sources[i].date.toString() + " | " + d.sources[i].name + '</a>'
    }
    return links;
}


var RowChart = function (facts, attribute, width, maxItems) {
    this.dim = facts.dimension(dc.pluck(attribute));
    dc.rowChart("#dc-chart-" + attribute)
        .dimension(this.dim)
        .group(this.dim.group().reduceCount())
        .data(function (d) { return d.top(maxItems); })
        .width(width)
        .height(maxItems * 22)
        .margins({ top: 0, right: 10, bottom: 20, left: 20 })
        .elasticX(true)
        .ordinalColors(['#9ecae1']) // light blue
        .labelOffsetX(5)
        .xAxis().ticks(4).tickFormat(d3.format(".2s"));
}


