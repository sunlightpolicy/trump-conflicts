
function timelinePopup(conflictId) {

    d3.json("data/ethics/" + conflictId + ".json", function(data) {
        makeTimelinePopup(data, conflictId);
    });
}


function makeTimelinePopup(ethicsData, conflictId) {

    var timeline = new TL.Timeline('timeline', 'data/timeline/' + conflictId + '.json', {
        ga_property_id: "UA-27829802-4",
        is_embed:true
    });
    
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
    //conflict.text(conflictData.name);

    var conflictDescription = d3.select('#conflictDescription');
    //conflictDescription.text(conflictData.description);

    
    d3.select('#timeline').selectAll("svg").remove();
  

    //addStories(mediaOutlets);
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