
var changeDateChart;
var sourceTypeChart;

var categoryChart;
var familyMemberChart;

var conflictingEntityChart;
var sourceChart;


//d3.json("data/conflicts.json", function (data) {
d3.json("data/stories.json", function (data) {
    data.forEach(function (d) {
        d.sourceType = "Office of Government Ethics";
        //if ((typeof(d.sources[0]) != "undefined") && (d.sources[0].name != "Office of Government Ethics"))
        //    d.sourceType = "Media";
        if (d.source != "Office of Government Ethics")
            d.sourceType = "Media";
        
        //d.source = "N/A";
        //if (typeof (d.sources[0]) != "undefined") {
        //    d.source = d.sources[0].name;

        //    if ((typeof (d.sources[1]) != "undefined") && (d.sources[1].name != d.source))
        //        d.source = "Multiple Sources";
        //}

        if (d.conflictingEntity == "")
            d.conflictingEntity = "N/A";

        d.description = d.category + " - " + d.description;
        //d.links = getLinks(d);
        d.links = getLink(d);

        d.dateChanged = new Date(d.dateChanged);
        d.sourceDate = new Date(d.sourceDate);
    });
    var facts = crossfilter(data);

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);

    var changeDateDim = facts.dimension(function (d) { return d.sourceDate; });
    var changeDateGroup = changeDateDim.group(d3.time.day);
    changeDateChart = dc.barChart("#dc-chart-changeDate")
        .dimension(changeDateDim)
        .group(changeDateGroup)
        .x(d3.time.scale().domain([new Date(2017, 2, 15), new Date(2018, 3, 31)]))
        .xUnits(d3.time.day)
        .width(420)
        .height(140)
        .margins({ top: 5, right: 30, bottom: 30, left: 50 })
        .elasticY(true)
        .filter([new Date(2017, 5, 25), new Date(2018, 3, 31)])
    changeDateChart.yAxis().ticks(5);
    changeDateChart.xAxis().ticks(5);

    var pieRadius = 70;
    var pieWidthAndHeight = 170;

    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue

    //var pieColors = ['#1f77b4', '#bd9e39', '#ad494a', '#637939'];
    //var pieColors = ['#f7f7f7', '#d9d9d9', '#bdbdbd', '#969696', '#636363', '#252525'];
    
    //var familyMemberDim = facts.dimension(dc.pluck('familyMember'));
    //familyMemberChart = dc.pieChart("#dc-chart-familyMember")
    //    .dimension(familyMemberDim)
    //    .group(familyMemberDim.group().reduceCount())
    //    .width(pieWidthAndHeight)
    //    .height(pieWidthAndHeight)
    //    .radius(pieRadius)
    //    .ordinalColors(pieColors)
    //    .on('filtered', showFilters);

    //var categoryDim = facts.dimension(dc.pluck('category'));
    //categoryChart = dc.pieChart("#dc-chart-category")
    //    .dimension(categoryDim)
    //    .group(categoryDim.group().reduceCount())
    //    .width(pieWidthAndHeight)
    //    .height(pieWidthAndHeight)
    //    .radius(pieRadius)
    //    .ordinalColors(pieColors)
    //    .on('filtered', showFilters);

    var col1Width = 240;
    var col2Width = 180;


    familyMemberChart = new RowChart(facts, "familyMember", col1Width, 6, 130);
    categoryChart = new RowChart(facts, "category", col2Width, 6, 130);
    categoryChart.filter("Active");


    sourceTypeChart = new RowChart(facts, "sourceType", 400, 2, 70);
    sourceTypeChart.filter("Media");

    conflictingEntityChart = new RowChart(facts, "conflictingEntity", col1Width, 400);
    sourceChart = new RowChart(facts, "source", col2Width, 30);
    

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
            //function (d) { return d.notes; },
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

function getLink(d) {
    return '<a href="' + d.link + '" target="_blank">' + d.sourceDate.toString() + " | " + d.source + '</a>'
}


function showFilters() {
    var filterStrings = [];
    var charts = dc.chartRegistry.list();
    charts.forEach(function (chart) {
        chart.filters().forEach(function (filter) {
            // Ugh, don't include date range for now, because I can't figure out how to get to underlying dates
            if (!Array.isArray(filter))
                filterStrings.push(filter);
        })
    })
    console.log(filterStrings)

    if (filterStrings.length == 0)
        filterString = "Showing all items in date range";
    else
        filterString = "Current Filters: " + filterStrings.join(', ');

    d3.select("#filters").text(filterString);
}


var RowChart = function (facts, attribute, width, maxItems, height) {

    // If height is supplied (very few items) use it, otherwise calculate
    if (!height)
        height = maxItems * 22;

    this.dim = facts.dimension(dc.pluck(attribute));
    var chart = dc.rowChart("#dc-chart-" + attribute)
        .dimension(this.dim)
        .group(this.dim.group().reduceCount())
        .data(function (d) { return d.top(maxItems); })
        .width(width)
        .height(height)
        .margins({ top: 0, right: 10, bottom: 20, left: 20 })
        .elasticX(true)
        .ordinalColors(['#9ecae1']) // light blue
        .labelOffsetX(5)
        .on('filtered', showFilters)
        .label(function (d) {
            return d.key + " " + d.value;
        });

    //    .Axis().ticks(4).tickFormat(d3.format(".2s"));

    return chart;
}


