var billion = 1000000000;

var appropriationTypeColors =
    ["#74C365", // light green 
    "#006600",  // dark green 
    "#007BA7"]; // blue


d3.csv("data/TrumpConflicts.csv", function (data) {
    data.forEach(function (d) {
        d.Id = +d.Id; 
        if (d.Category == "active")
            d.Category = "Active";
        if (d.Category == "potential")
            d.Category = "Potential";
        if (d.Category == "resolved")
            d.Category = "Resolved";
    });
    var facts = crossfilter(data);

    new RowChart(facts, "FamilyMember", 300, 6);
    new RowChart(facts, "ConflictingEntity", 300, 400);
    new RowChart(facts, "Category", 300, 3);

    dataTable = dc.dataTable("#dc-chart-table");

    var tableDim = facts.dimension(function(d) { return +d.Id; });

    dataTable
        .dimension(tableDim)
        .group(function(d) {})
        .showGroups(false)
        .size(5)
        //.size(xf.size()) //display all data
        .columns([
            function(d) { return d.Id; },
            function(d) { return d.Description; },
            function(d) { return d.FamilyMember; },          
            function(d) { return d.ConflictingEntity; },
            function(d) { return d.Category; },
            function(d) { return d.Source1; }
            //function(d) { return '<a href="https://scholar.google.fr/scholar?q=' + d.DOI + '" target="_blank">' + d.DOI + '</a>' },
        ])
    .sortBy(function(d){ return +d.Id; })
    .order(d3.ascending);
   
    dc.renderAll();
});

// 07 constructor function for row charts
var RowChart = function (facts, attribute, width, maxItems) {
    this.dim = facts.dimension(dc.pluck(attribute));
    debugger;
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


