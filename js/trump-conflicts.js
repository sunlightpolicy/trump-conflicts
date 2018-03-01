
var changeDateChart;

var categoryChart;
var familyMemberChart;

var conflictingEntityChart;
var sourceChart;


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
        d.dateChanged = new Date(d.dateChanged);
    });
    var facts = crossfilter(data);

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);

    var changeDateDim = facts.dimension(function (d) { return d.dateChanged; });
    var changeDateGroup = changeDateDim.group(d3.time.day);
    changeDateChart = dc.barChart("#dc-chart-changeDate")
        .dimension(changeDateDim)
        .group(changeDateGroup)
        .x(d3.time.scale().domain([new Date(2017, 5, 15), new Date(2018, 3, 31)]))
        .xUnits(d3.time.days)
        .width(420)
        .height(140)
        .margins({ top: 5, right: 30, bottom: 30, left: 50 })
        .elasticY(true)
        .filter([new Date(2017, 5, 25), new Date(2018, 2, 31)])
    changeDateChart.yAxis().ticks(5);
    changeDateChart.xAxis().ticks(5);

    var pieRadius = 70;
    var pieWidthAndHeight = 170;

    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue

    var categoryDim = facts.dimension(dc.pluck('category'));
    categoryChart = dc.pieChart("#dc-chart-category")
        .dimension(categoryDim)
        .group(categoryDim.group().reduceCount())
        .width(pieWidthAndHeight)
        .height(pieWidthAndHeight)
        .radius(pieRadius)
        .ordinalColors(pieColors);

    var familyMemberDim = facts.dimension(dc.pluck('familyMember'));
    familyMemberChart = dc.pieChart("#dc-chart-familyMember")
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

    conflictingEntityChart = new RowChart(facts, "conflictingEntity", 240, 400);
    sourceChart = new RowChart(facts, "source", 160, 30);
    

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
    var chart = dc.rowChart("#dc-chart-" + attribute)
        .dimension(this.dim)
        .group(this.dim.group().reduceCount())
        .data(function (d) { return d.top(maxItems); })
        .width(width)
        .height(maxItems * 22)
        .margins({ top: 0, right: 10, bottom: 20, left: 20 })
        .elasticX(true)
        .ordinalColors(['#9ecae1']) // light blue
        .labelOffsetX(5)

    //chart
    //    .Axis().ticks(4).tickFormat(d3.format(".2s"));

    return chart;
}


