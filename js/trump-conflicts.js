

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

        d.link = getHeadlineLink(d);

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
    
    var leftWidth = 430;

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
    
    var col1Width = leftWidth / 2;
    var col2Width = leftWidth / 2;
    
    familyMemberChart = new RowChart(facts, "familyMember", col1Width, 6, 110);
    categoryChart = new RowChart(facts, "category", col2Width, 6, 110);
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
            return conflictHeader(d) + ethicsPopupLink(d);
        })  
        .size(9)  // Remove this and add scroll!!
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
    return "<b>" + d.conflict + "</b> <em>(" + d.familyMember + " / " + d.category + ")</em> " + d.description;
}

function ethicsPopupLink(d) {
    let link = ""; 
    if (d.hasEthics)
        //link = " <a href=\"#\" onclick=\"ethicsPopup(" + d.conflictId + "); return false\"><b>Ethics Report</b></a>"
        link = " <a href=\"#\" onclick=\"timeline(" + d.conflictId + "); return false\"><b>Timeline</b></a>"
    return link;
}

function ethicsPopup(conflictId) {

    d3.json("data/ethics/" + conflictId + ".json", function (err, data) {
        var span = document.getElementsByClassName("close")[0];
        span.onclick = function () {
            modal.style.display = "none";
        }
        window.onclick = function (event) {
            if (event.target == modal)
                modal.style.display = "none";
        }
        console.log(data);

        let modal = document.getElementById('ethicsModal');
        modal.style.display = "block";

        var conflict = d3.select('#conflict');
        conflict.text(data.conflict);

        var conflictDescription = d3.select('#conflictDescription');
        conflictDescription.text(data.conflictDescription);

        addEthicsDocuments(d3.select('#ethicsModalBody'), data);
    });
}

function timeline(conflictId) {
    d3.json("data/media/" + conflictId + ".js", function (err, conflictData) {
        
        var span = document.getElementsByClassName("close")[0];
        span.onclick = function () {
            modal.style.display = "none";
        }
        window.onclick = function (event) {
            if (event.target == modal)
                modal.style.display = "none";
        }
        //console.log(data);

        let modal = document.getElementById('ethicsModal');
        modal.style.display = "block";

        var conflict = d3.select('#conflict');
        conflict.text(conflictData.name);

        var conflictDescription = d3.select('#conflictDescription');
        conflictDescription.text(conflictData.description);

        //addEthicsDocuments(d3.select('#ethicsModalBody'), data);
        const ONE_HOUR = 60 * 60 * 1000,
        ONE_DAY = 24 * ONE_HOUR,
        ONE_WEEK = 7 * ONE_DAY,
        ONE_MONTH = 30 * ONE_DAY,
        SIX_MONTHS = 6 * ONE_MONTH;
        TWO_YEARS = 24 * ONE_MONTH;
  
  var data = [],
    start = new Date('2016-06-02T20:14:22.691Z'),
    today = new Date('2018-06-08T17:59:06.134Z');
  
  debugger;
  var mediaOutlets = conflictData.mediaOutlets
  for (var x in mediaOutlets) { 
    data[x] = {};
    data[x].name = mediaOutlets[x].name;
    data[x].data = [];
    for (var y in mediaOutlets[x].data) {
      data[x].data.push({});
      data[x].data[y].date = new Date(mediaOutlets[x].data[y].date);
      data[x].data[y].details = mediaOutlets[x].data[y].details;
    }
    //$('#timeline-selectpicker').append("<option>" + data[x].name + "</option>");
    data[x].display = true;
  }
  //$('#timeline-selectpicker').selectpicker('selectAll');
  
  var timeline = d3.chart.timeline()
    .end(today)
    .start(today - TWO_YEARS)
    .minScale(ONE_WEEK / ONE_MONTH)
    .maxScale(ONE_WEEK / ONE_HOUR)
    .slider(false)
    .lineHeight(30)
    //.eventPopover("HI")
    .eventClick(function(el) {
      var table = '<table class="table table-striped table-bordered">';
      if(el.hasOwnProperty("events")) {
        table = table + '<thead>This is a group of ' + el.events.length + ' events starting on '+ el.date + '</thead><tbody>';
        table = table + '<tr><th>Date</th><th>Event</th><th>Object</th></tr>';
        for (var i = 0; i < el.events.length; i++) {
          table = table + '<tr><td>' + el.events[i].date + ' </td> ';
          for (var j in el.events[i].details) {
            table = table +'<td> ' + el.events[i].details[j] + ' </td> ';
          }
          table = table + '</tr>';
        }
        table = table + '</tbody>';
      } else {
        table = table + 'Date: ' + el.date + '<br>';
        for (i in el.details) {
          table = table + i.charAt(0).toUpperCase() + i.slice(1) + ': ' + el.details[i] + '<br>';
        }
      }
      $('#legend').html(table);
  
    });
  /* if(countNames(data) <= 0) {
    timeline.labelWidth(60);
  } */
  
  
  // Keep this
  var element = d3.select('#timeline').append('div').datum(data.filter(function(eventGroup) {
    return eventGroup.display === true;
  }));
  timeline(element);
    });
}

function addEthicsDocuments(modal, data) {
    modal.selectAll("h4")
        .data(data.familyMemberBusinessWithEthicsList)
        .enter()

        .append("h4")
        .text(function (d) { return d.business + " / " + d.familyMember + " / " + d.conflictStatus; })

        .append("p")
        .text(function (d) { return d.description; })
        .style("font-size", "13px")

        // This breaks if there are no ethics docs, and will only show the first if there are more than one!
        .append("a")
        .attr("href", function (d) { return d.ethicsDocuments[0].link; })
        .text(function (d) { return " " + d.ethicsDocuments[0].name; })

        .append("hr");
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
            return d.key + " " + d.value;
        });

    //    .Axis().ticks(4).tickFormat(d3.format(".2s"));

    return chart;
}


