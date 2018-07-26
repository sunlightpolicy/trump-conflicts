var changeDateChart;
var sourceTypeChart;

var categoryChart;
var familyMemberChart;

var conflictChart;
var mediaOutletChart;

var searchDim;

//d3.json("data/stories.json", function (err, data) {
d3.json("data/conflicts.json", function (err, data) {
    data.forEach(function (d) {
        //d.stories = +d.stories;
    });

    console.table(data);
    var facts = crossfilter(data);

    searchDim = facts.dimension(function (d) {
        //return d.mediaOutlet.toLowerCase() + " " + d.conflict.toLowerCase();
        return d.name.toLowerCase();
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
    
    var pieColors =
        ["#74C365", // light green 
        "#006600",  // dark green 
        "#007BA7"]; // blue
    
    var col1Width = leftWidth / 2;
    var col2Width = leftWidth / 2;
    
    familyMemberChart = new RowChart(facts, "familyMember", leftWidth, 6, 110);
    //categoryChart = new RowChart(facts, "category", col2Width, 6, 110);
    //categoryChart.filter("Active");

    //sourceTypeChart = new RowChart(facts, "sourceType", leftWidth, 2, 70);
    //sourceTypeChart.filter("Media");

    //conflictChart = new RowChart(facts, "conflict", leftWidth, 50);
    //mediaOutletChart = new RowChart(facts, "mediaOutlet", col2Width, 30);
  
/*     var conflictDim = facts.dimension(function(d) {
        return d.familyMember;
    });

    //var conflictDim = facts.dimension(dc.pluck("name"));
    dc.dataGrid("dc-chart-dataGrid")
        .dimension(conflictDim)
        .group(function (d) {
            return d.familyMember;
        })
        .html(function (d) {
             return "HELLO"; 
        })
        .htmlGroup (function (d) { return '<h2>HELLO</h2>'})
        .size(1000) 
        .order(function (d) {
            return d.familyMember;
        }) */
       
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
            //return 10000 - d.stories;
        })
        .size(1000)
        .order(d3.descending);
    

    dc.renderAll();    
});

/* function conflictHeader(d) {
    return "<b>" + d.conflict + "</b> <em>(" + d.familyMember + " / " + d.category + ")</em> "; // + d.description;
} */


function conflictResult(d) {
    let pad = "0000"
    let ans = pad.substring(0, pad.length - d.stories.length) + d.stories;

    let classes =  "class='conflict-summary' " + ans + " onclick='timelinePopup(\"" + d.slug + "\")' ";
    
    let title = "<h4 class='conflict-title'>" + d.name + "</h4>";

    let description = "";
    if (d.description)
        description = "<p class='conflict-description'>" + d.description + "</p>";

    let stories = "<span class='conflict-stories'>No media accounts of this conflict</span>";
    if (d.stories == 1)
        stories = "<span class='conflict-stories'>" + d.stories + " media account, from " + d.lastStory + "</span>";
    if (d.stories > 1)
        stories = "<span class='conflict-stories'>" + d.stories + " media accounts, most recent " + d.lastStory + "</span>";


    return "<div " + classes + ">" + title + description + stories + "</div>";
}

/* function ethicsPopupLink(d) {
    let link = ""; 
    //link = " <a href=\"#\" onclick=\"timelinePopup(" + d.conflictId + "); return false\"><b>Timeline</b></a>"
    link = " <a href=\"#\" onclick=\"timelinePopup('" + d.slug + "'); return false\"><b>Details</b></a>"
    return link;
} */

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


