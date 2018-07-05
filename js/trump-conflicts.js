var changeDateChart;
var sourceTypeChart;

var categoryChart;
var familyMemberChart;

var conflictChart;
var mediaOutletChart;

var searchDim;


d3.json("data/stories.json", function (err, data) {
    data.forEach(function (d) {

        d.sourceType = "Office of Government Ethics";

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

        d.link = getMediaOutletAndHeadline(d);

        d.dateChanged = new Date(d.dateChanged);
        d.sourceDate = new Date(d.sourceDate);
    });

    //console.table(data);
    var facts = crossfilter(data);

    searchDim = facts.dimension(function (d) {
        return d.mediaOutlet.toLowerCase() + " " + d.conflict.toLowerCase();
    });
    
    d3.select("#search-input").on('keyup', function (event) {
        searchTerm = document.getElementById("search-input").value;
        setword(searchTerm);
    });

    
    function setword(wd) {
        if (wd.length < 3) {
            searchDim.filter(null);
            dc.redrawAll();  
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

    var totalGroup = facts.groupAll().reduce(
        function (p, v) { // add function
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
    
    var leftWidth = 400;

    var changeDateDim = facts.dimension(function (d) { return d.sourceDate; });
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
    changeDateChart.xAxis().ticks(5);
    
    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue
    
    var col1Width = leftWidth / 2;
    var col2Width = leftWidth / 2;
    
    //familyMemberChart = new RowChart(facts, "familyMember", col1Width, 6, 110);
    //categoryChart = new RowChart(facts, "category", col2Width, 6, 110);
    //categoryChart.filter("Active");

    //sourceTypeChart = new RowChart(facts, "sourceType", leftWidth, 2, 70);
    //sourceTypeChart.filter("Media");

    conflictChart = new RowChart(facts, "conflict", leftWidth, 50);
    //mediaOutletChart = new RowChart(facts, "mediaOutlet", col2Width, 30);
    
    dataTable = dc.dataTable("#dc-chart-table");

    var tableDim = facts.dimension(function(d) { return +d.Id; });

    dataTable
        .dimension(tableDim)
        .group(function (d) {
            return conflictHeader(d) + ethicsPopupLink(d);
        })  
        .size(20)  // Remove this and add scroll!!
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

function conflictHeader(d) {
    return "<b>" + d.conflict + "</b> <em>(" + d.familyMember + " / " + d.category + ")</em> "; // + d.description;
}

function ethicsPopupLink(d) {
    let link = ""; 
    //if (d.hasEthics)
        //link = " <a href=\"#\" onclick=\"ethicsPopup(" + d.conflictId + "); return false\"><b>Ethics Report</b></a>"
        link = " <a href=\"#\" onclick=\"timelinePopup(" + d.conflictId + "); return false\"><b>Timeline</b></a>"
    return link;
}

function dateToYMD(date) {
    var d = date.getDate();
    var m = date.getMonth() + 1; // Month from 0 to 11
    var y = date.getFullYear();
    return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
}

function getMediaOutletAndHeadline(d) {
    var text = "<b>" + d.mediaOutlet + "</b>";
    if (d.headline != "")
        text = text + " / " + d.headline;

    return text;
    //return '<a href="' + d.link + '" target="_blank">' + text + '</a>';
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
    // console.log(filterStrings)

    if (filterStrings.length == 0)
        filterString = "Showing all items in date range";
    else
        filterString = "Current Filters: " + filterStrings.join(', ');

    //d3.select("#filters").text(filterString);
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
            return d.key; // + " " + d.value;
        });

    //  .Axis().ticks(4).tickFormat(d3.format(".2s"));

    return chart;
}


