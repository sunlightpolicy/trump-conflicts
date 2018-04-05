
var changeDateChart;
var sourceTypeChart;

var categoryChart;
var familyMemberChart;

var conflictChart;
var mediaOutletChart;

var searchDim;


//d3.json("data/conflicts.json", function (data) {
d3.json("data/stories3.json", function (err, data) {
    data.forEach(function (d) {

        d.sourceType = "Office of Government Ethics";
        //if ((typeof(d.sources[0]) != "undefined") && (d.sources[0].name != "Office of Government Ethics"))
        //    d.sourceType = "Media";
        if (d.mediaOutlet != "Office of Government Ethics")
            d.sourceType = "Media";
        
        //d.source = "N/A";
        //if (typeof (d.sources[0]) != "undefined") {
        //    d.source = d.sources[0].name;

        //    if ((typeof (d.sources[1]) != "undefined") && (d.sources[1].name != d.source))
        //        d.source = "Multiple Sources";
        //}

        if (d.conflict == "")
            d.conflict = "N/A";

        d.link = getHeadlineLink(d);

        d.dateChanged = new Date(d.dateChanged);
        d.sourceDate = new Date(d.sourceDate);
    });
    var facts = crossfilter(data);

    searchDim = facts.dimension(function (d) {
        return d.mediaOutlet.toLowerCase() + " " + d.conflict.toLowerCase();
    });
    
    //d3.select("#search-input").on('click', function () {
    //    console.log(document.getElementById("search-input").value);
    //    setword(document.getElementById("search-input").value);
    //});

    d3.select("#search-input").on('keyup', function (event) {
        searchTerm = document.getElementById("search-input").value;
        //if (event.keyCode == 13) {
        setword(searchTerm);
        console.log(searchTerm);
    });

    
    function setword(wd) {
        if (wd.length < 3) {
            searchDim.filter(null);
            dc.redrawAll();  
            console.log("Too Short");
            return;
        }
        
        var s = wd.toLowerCase();
        searchDim.filter(function (d) {
            return d.indexOf(s) !== -1;
        });

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

    // 01 group for grand total 
    var totalGroup = facts.groupAll().reduce(
        function (p, v) { // add finction
            return p += v.amount;
        },
        function (p, v) { // subtract function
            return p -= v.amount;
        },
        function () { return 0 } // initial function
    );

    var all = facts.groupAll();
    dc.dataCount('.dc-data-count')
        .dimension(facts)
        .group(all);

    //dc.numberDisplay("#dc-chart-total")
    //    .group(totalGroup)
    //    .valueAccessor(function (d) {
    //        return d / billion;
    //    })
    //    .formatNumber(function (d) { return Math.round(d) + " Billion"; });
    
    var leftWidth = 540;

    var changeDateDim = facts.dimension(function (d) { return d.sourceDate; });
    var changeDateGroup = changeDateDim.group(d3.time.day);
    changeDateChart = dc.barChart("#dc-chart-changeDate")
        .dimension(changeDateDim)
        .group(changeDateGroup)
        //.x(d3.time.scale().domain([new Date(2013, 2, 15), new Date(2018, 3, 31)]))
        .x(d3.time.scale().domain([new Date(2016, 2, 15), new Date(2018, 3, 31)]))
        .xUnits(d3.time.day)
        .width(leftWidth)
        .height(140)
        .margins({ top: 5, right: 30, bottom: 30, left: 50 })
        .elasticY(true)
        .filter([new Date(2017, 9, 25), new Date(2018, 2, 31)]) // Months are zero based
    changeDateChart.yAxis().ticks(5);
    changeDateChart.xAxis().ticks(5);
    
    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue
    
    var bootstrapCols = 12;
    var col1Width = leftWidth * (7 / bootstrapCols);
    var col2Width = leftWidth * (5 / bootstrapCols);

    console.log(7 / bootstrapCols);
    console.log(leftWidth * (7 / bootstrapCols));
    console.log(leftWidth * (5 / bootstrapCols));

    //var col1Width = 280;
    //var col2Width = 180;

    familyMemberChart = new RowChart(facts, "familyMember", col1Width, 6, 130);
    categoryChart = new RowChart(facts, "category", col2Width, 6, 130);
    categoryChart.filter("Active");

    sourceTypeChart = new RowChart(facts, "sourceType", leftWidth, 2, 70);
    sourceTypeChart.filter("Media");

    conflictChart = new RowChart(facts, "conflict", col1Width, 10);
    mediaOutletChart = new RowChart(facts, "mediaOutlet", col2Width, 10);
    
    dataTable = dc.dataTable("#dc-chart-table");

    var tableDim = facts.dimension(function(d) { return +d.Id; });

    dataTable
        .dimension(tableDim)
        .group(function (d) {
            return "<b>" + d.conflict + "</b> <em>(" + d.familyMember + " / " + d.category + ")</em> " +
                d.description + d.conflictId + "  <a href=\"#\" onclick=\"ethicsPopup(" + d.conflictId + "); return false\"><b>Ethics Report</b></a>"
        })  
        //.showGroups(false)
        .size(50)
        .columns([
            function (d) { return dateToYMD(d.sourceDate); },
            function(d) { return d.link; }
        ])
        .sortBy(function (d) { return d.conflict + dateToYMD(d.sourceDate); })
        .order(d3.ascending)
        .renderlet(function (table) {
            table.selectAll(".dc-table-group").classed("info", true);
        });

    dc.renderAll();    
});

function ethicsPopup(conflictId) {
    d3.json("data/ethics/" + conflictId + ".json", function (err, data) {
        let modal = document.getElementById('ethicsModal');
        modal.style.display = "block";
        
        var table = d3.select('#ethicsModal').append('table')
        var thead = table.append('thead')
        var tbody = table.append('tbody');

        var span = document.getElementsByClassName("close")[0];
        span.onclick = function () {
            modal.style.display = "none";
        }
        window.onclick = function (event) {
            if (event.target == modal)
                modal.style.display = "none";
        }

        console.log(data);
    });
}

function dateToYMD(date) {
    var d = date.getDate();
    var m = date.getMonth() + 1; // Month from 0 to 11
    var y = date.getFullYear();
    return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
}

function clearAll() {
    console.log("Clear ALL");

    searchDim.filter(null); // clear text too?
    document.getElementById("search-input").value = "";

    dc.filterAll();
    dc.renderAll();
}


function getHeadlineLink(d) {
    var text = "<b>" + d.mediaOutlet + "</b>";
    if (d.headline != "")
        text = text + " / " + d.headline;

    return '<a href="' + d.link + '" target="_blank">' + text + '</a>';
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


