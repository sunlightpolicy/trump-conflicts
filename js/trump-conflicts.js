

var appropriationTypeColors =
    ["#74C365", // light green 
    "#006600",  // dark green 
    "#007BA7"]; // blue


d3.json("data/conflicts.json", function (data) {
    data.forEach(function (d) {
        d.sourceType = "Office of Government Ethics";
        if ((typeof(d.sources[0]) != "undefined") && (d.sources[0].name != "Office of Government Ethics"))
            d.sourceType = "Media";

        d.source = "None";
        if (typeof (d.sources[0]) != "undefined") {
            d.source = d.sources[0].name;

            if ((typeof (d.sources[1]) != "undefined") && (d.sources[1].name != d.source))
                d.source = "Multiple";
        }
    });
    var facts = crossfilter(data);

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);


    var categoryDim = facts.dimension(dc.pluck('category'));
    dc.pieChart("#dc-chart-category")
        .dimension(categoryDim)
        .group(categoryDim.group().reduceCount())
        .width(200)
        .height(200)
        .radius(80)
        .ordinalColors(appropriationTypeColors);

    var sourceTypeDim = facts.dimension(dc.pluck('sourceType'));
    dc.pieChart("#dc-chart-sourceType")
        .dimension(sourceTypeDim)
        .group(sourceTypeDim.group().reduceCount())
        .width(200)
        .height(200)
        .radius(80)
        .ordinalColors(appropriationTypeColors)


    //new RowChart(facts, "familyMember", 200, 6);
    new RowChart(facts, "conflictingEntity", 240, 400);
    new RowChart(facts, "source", 160, 40);
    //new RowChart(facts, "category", 300, 3);
    //new RowChart(facts, "sourceType", 300, 2);
    
    dataTable = dc.dataTable("#dc-chart-table");

    var tableDim = facts.dimension(function(d) { return +d.Id; });

    dataTable
        .dimension(tableDim)
        .group(function(d) {})
        .showGroups(false)
        .size(20)
        //.size(xf.size()) //display all data
        .columns([
            //function(d) { return d.Id; },
            function(d) { return d.description; },
            function(d) { return d.familyMember; },          
            function(d) { return d.conflictingEntity; },
            //function (d) { return d.category; },
            function (d) { return d.source; }
        ])
    //.sortBy(function(d){ return +d.Id; })
    .order(d3.ascending);
   
    dc.renderAll();
});



var RowChart = function (facts, attribute, width, maxItems) {
    this.dim = facts.dimension(dc.pluck(attribute));
    //debugger;
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


