var changeDateChart;
var sourceTypeChart;

var categoryChart;
var familyMemberChart;

var conflictChart;
var mediaOutletChart;

var searchDim;


d3.json("data/conflicts.json", function (err, data) {

    console.table(data);
    var facts = crossfilter(data);

    searchDim = facts.dimension(function (d) {
        return d.name.toLowerCase();
    });
    
    d3.select("#search-input").on('keyup', function (event) {
        searchTerm = document.getElementById("search-input").value;
        setword(searchTerm);
    });

    window.addEventListener('keyup', (e) => {
        if (e.keyCode == 27) {
            console.log("Escape Key Pressed");
        }   
    });

    
    function setword(wd) {
        if (wd.length < 3) {

            if (wd.length == 0)
                showFilters();

            searchDim.filter(null);
            dc.redrawAll();  
            return;
        }
        
        var s = wd.toLowerCase();
        searchDim.filter(function (d) {
            return d.indexOf(s) !== -1;
        });

        showFilters();
        dc.redrawAll();  
        //$(".resetall").attr("disabled", false);
        //throttle();
        //        dc.redrawAll();

        //var throttleTimer;
        //function throttle() {
        //    window.clearTimeout(throttleTimer);
        //    throttleTimer = window.setTimeout(function () {
        //        console.log("redraw");
        //        dc.redrawAll();
        //    }, 250);
        //}
    }

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);    

    /* var changeDateDim = facts.dimension(function (d) { return d.sourceDate; });
    var changeDateGroup = changeDateDim.group(d3.time.day);
    changeDateChart = dc.barChart("#dc-chart-changeDate")
        .dimension(changeDateDim)
        .group(changeDateGroup)
        //.x(d3.time.scale().domain([new Date(2013, 2, 15), new Date(2018, 3, 31)]))
        .x(d3.time.scale().domain([new Date(2016, 2, 15), new Date(2018, 6, 15)]))
        .xUnits(d3.time.day)
        .width(leftWidth)
        .height(140)
        .margins({ top: 15, right: 20, bottom: 20, left: 40 })
        .elasticY(true)
        .filter([new Date(2016, 2, 25), new Date(2018, 6, 10)]) // Months are zero based
    changeDateChart.yAxis().ticks(5);
    changeDateChart.xAxis().ticks(5); */
    
    var col1Width = 300;
    
    familyMemberChart = new DivChart(facts, "familyMember");

    statusChart = new RowChart(facts, "status", col1Width, 6, 100);
    //statusChart.filter("Active");
     
    dataTable = dc.dataTable("#dc-chart-dataGrid");
    var tableDim = facts.dimension(function(d) { return +d.Id; });
    dataTable
        .dimension(tableDim)
        .group(function (d) {
            return conflictResult(d); 
        })
        .sortBy(function(d) {
            var pad = "0000"
            var ans = pad.substring(0, pad.length - d.stories.length) + d.stories;
            return ans;
        })
        .size(1000)
        .order(d3.descending);

    dc.renderAll();    
});

function clearSearch() {
    console.log("Clear Search");
    searchDim.filter(null);
    document.getElementById("search-input").value = "";
}

function conflictResult(d) {
    let pad = "0000"
    let ans = pad.substring(0, pad.length - d.stories.length) + d.stories;

    let classes =  "class='conflict-summary' " + ans + " onclick='conflictPopup(\"" + d.slug + "\")' ";
    
    let title = "<h4 class='conflict-title'>" + d.name + "</h4>";

    let stories = "<span class='conflict-stories'>No media accounts of this conflict</span>";
    if (d.stories == 1)
        stories = "<span class='conflict-stories'>" + d.stories + " media account, from " + d.lastStory + "</span>";
    if (d.stories > 1)
        stories = "<span class='conflict-stories'>" + d.stories + " media accounts, most recent " + d.lastStory + "</span>";

    let description = "";
    if (d.description)
        description = "<p class='conflict-description'>" + d.description + "</p>";

    return "<div " + classes + ">" + title + stories + description + "</div>";
}

function dateToYMD(date) {
    var d = date.getDate();
    var m = date.getMonth() + 1; // Month from 0 to 11
    var y = date.getFullYear();
    return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
}

function clearAll() {
    searchDim.filter(null); // clear text too?
    document.getElementById("search-input").value = "";

    dc.filterAll();
    dc.renderAll();
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
    var search = document.getElementById("search-input").value;
    if (search.length < 4)
        search = "";
    else
        search =  ' containing "' + search + '"';
    filterString = filterStrings.join(', ') + " " + search;

    d3.select("#results").text(filterString);
}

var DivChart = function (facts, attribute) {
    this.dim = facts.dimension(dc.pluck("familyMember"));
    var chart = dc.divChart("#dc-chart-" + attribute)
        .dimension(this.dim)
        .group(this.dim.group().reduceCount())
        .data(function (d) { return d.top(50); })
        .margins({ top: 0, right: 10, bottom: 20, left: 20 })
        .on('filtered', showFilters)
        .label(function (d) {
            return d.key; 
        });

    return chart;
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
            return d.key; 
        });

    //  .Axis().ticks(4).tickFormat(d3.format(".2s"));

    return chart;
}


