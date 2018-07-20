function loadSection() {
    var slug = location.hash.replace("#", "");
    alert(slug);
    if (slug != "")
        timelinePopup(slug);
}


function timelinePopup(conflictSlug) {
    d3.json("data/conflicts/" + conflictSlug + ".json", data => buildConflictPage(data)); 

    showConflictPage();
    
    history.pushState(null, null, "index.html#" + conflictSlug);
}

function buildConflictPage(data) {
    var name = "No conflict name - ethics data for conflict not found";
    var description = "No conflict description - ethics data for conflict not found";

    if (data != null) {
        name = data.name;
        description = data.description;
    }

    var conflict = d3.select('#conflict');
    conflict.text(name);

    var conflictDescription = d3.select('#conflictDescription');
    conflictDescription.text(description);

    addStories(data);
    addEthics(data);
}

function addStories(data) {
    var html = h2("Media Accounts");
    html += "<hr>"; 

    var cols = [
        {"header": "Date",     "class": "td-date", "field": "date"}, 
        {"header": "Outlet",   "class": "td-text", "field": "mediaOutlet"},
        {"header": "Headline", "class": "td-link", "field": "hyperlink"}
    ];
    
    data.stories.forEach(story => {
        story.hyperlink = "<a href='" + story.link + "' target='_blank'>" + story.headline + "</a>";     
        story.date = dateToYmd(story.sourceDate);     
    }); 
    html += makeTable(cols, data.stories);

    d3.select('#stories').html(html);
}

function dateToYmd(dateString) {
    let date = new Date(dateString);

    var d = date.getDate();
    var m = date.getMonth() + 1; // Month from 0 to 11
    var y = date.getFullYear();
    return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
}

function addEthics(data) {
    if (data == null)
        return;

    var html = h2("Ethics Disclosures");
    html += "<hr>"; 

    var cols = [
        {"header": "Owner",      "class": "td-text",    "field": "owner"}, 
        {"header": "Percentage", "class": "td-percent", "field": "percentage"}
    ];

    let ethics = data.ethics[0];
    ethics.familyMemberEthics.forEach(bus => {
        html += h4(bus.business + " / " + bus.familyMember + " / " + bus.conflictStatus);
        html += p(bus.description);
        html += makeTable(cols, bus.ownerships);
    });
    d3.select('#ethics').html(html);
}


function showConflictPage() {
    //d3.select('#timeline').selectAll("svg").remove();

    //var timeline = new TL.Timeline('timeline', 'data/timeline/' + conflictId + '.json', {
    //    is_embed:true
    //});
    //timeline._el.container.offsetHeight = 400;

    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
        history.pushState(null, null, "index.html");
    }
    window.onclick = function (event) {
        if (event.target == modal)
            modal.style.display = "none";
    }
    let modal = document.getElementById('ethicsModal');
    modal.style.display = "block";
}
