
function timelinePopup(conflictId) {
    queue()
        .defer(d3.json, "data/media/" + conflictId + ".json")
        .defer(d3.json, "data/ethics/" + conflictId + ".json")
        .await(makeTimelinePopup);
}


function makeTimelinePopup(err, conflictData, ethicsData) {
    
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
        .slider(false)  // Scale thing to the right
        .context(false) // Brush below the timeline
        .marker(false)  // Floating popup under mouse with date range
        .lineHeight(15)
        .eventClick(popup());
    
    d3.select('#timeline').selectAll("svg").remove();
    var element = d3.select('#timeline')
        .append('div')
        .datum(data.filter(function(eventGroup) {
            return eventGroup.display === true;
    }));
    timeline(element);    

    addStories(mediaOutlets);
    addEthics(ethicsData);
}

function addStories(mediaOutlets) {
    console.table(mediaOutlets);
 
    var table = '';
    mediaOutlets.forEach(pub => {
        pub.data.forEach(story => {
            table = 
                table + 
                '<tr>' +
                    '<td class="conflict-col-1">' + dateToYMD(new Date(story.date)) + '</td>' +
                    '<td class="conflict-col-2"><a href="' + story.details.link + '" target="_blank"><b>' + pub.name + '</b> / ' + story.details.headline + '</a><td>' + 
                '</tr>';
        });
    });
    table = '<table style="width:90%">' + table + '</table>'; 
    d3.select('#stories').html('<h3>Media Accounts</h3>' + table);
}


function addStories(mediaOutlets) {
    console.table(mediaOutlets);
 
    var table = '';
    mediaOutlets.forEach(pub => {
        pub.data.forEach(story => {
            table = 
                table + 
                '<tr>' +
                    '<td class="conflict-col-1">' + dateToYMD(new Date(story.date)) + '</td>' +
                    '<td class="conflict-col-2"><a href="' + story.details.link + '" target="_blank"><b>' + pub.name + '</b> / ' + story.details.headline + '</a><td>' + 
                '</tr>';
        });
    });
    table = '<table style="width:90%">' + table + '</table>'; 
    d3.select('#stories').html('<h3>Media Accounts</h3>' + table);
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


function addEthics(data) {

    d3.select('#ethics').html('');

    d3.select('#ethics')
        .append('h3')
        .text('Ethics Disclosures')
        .append('hr')

    d3.select('#ethics')
        .selectAll('h4')
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


// For Timeline
function popup(el) {
    /*     var table = '<table class="table table-striped table-bordered">';
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
    
        $('#legend').html(table); */
    } 