
function timelinePopup(conflictId) {
    var ethicsData = d3.json("data/ethics/" + conflictId + ".json", data => addHeader(data)); 

    makeTimelinePopup(conflictId);
}

function addHeader(data) {
    var name = "No conflict name - ethics data for conflict not found";
    var description = "No conlfict description - ethics data for conflict not found";

    if (data != null) {
        name = data.conflict;
        description = data.conflictDescription;
    }

    var conflict = d3.select('#conflict');
    conflict.text(name);

    var conflictDescription = d3.select('#conflictDescription');
    conflictDescription.text(description);

    addEthics(data);
    return data;
}


function makeTimelinePopup(conflictId) {
    //d3.select('#timeline').selectAll("svg").remove();

    var timeline = new TL.Timeline('timeline', 'data/timeline/' + conflictId + '.json', {
        is_embed:true
    });
    timeline._el.container.offsetHeight = 400;

    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
    }
    window.onclick = function (event) {
        if (event.target == modal)
            modal.style.display = "none";
    }
    let modal = document.getElementById('ethicsModal');
    modal.style.display = "block";
}


function addEthics(data) {

    var cols = [
        {"header": "Owner",      "class": "td-text",    "field": "owner"}, 
        {"header": "Percentage", "class": "td-percent", "field": "percentage"}
    ];

    var ethicsHtml = h2("Ethics Disclosures");
    
    data.familyMemberBusinessWithEthicsList.forEach(bus => {
        ethicsHtml += h4(bus.business + " / " + bus.familyMember + " / " + bus.conflictStatus);
        ethicsHtml += p(bus.description);
        ethicsHtml += makeTable(cols, bus.ownerships);
    });
    
    d3.select('#ethics').html(ethicsHtml);
    return;


    if (data == null)
        return;

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

        .append("p")
        .data(data.familyMemberBusinessWithEthicsList.ownerships[0].owner);
        //.append("hr"); 
}


// Not used
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

// Not used - story links are in Timeline now
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