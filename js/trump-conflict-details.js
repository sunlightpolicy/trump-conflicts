function loadSection() {
    var slug = location.hash.replace("#", "");
    if (slug != "")
        conflictPopup(slug);    
}

function backOrForward() {
    var slug = location.hash.replace("#", "");
    if (slug != "")
        timelinePopup(slug);
    else
        document.getElementById('ethicsModal').style.display = "none";       
}


function conflictPopup(conflictSlug) {
    let dataUri = "data/conflicts/" + conflictSlug + ".json";

    d3.json(dataUri, data => buildConflictPage(data)); 
    
    var conflictJson = d3.select('#conflict-json');
    conflictJson.attr("xlink:href", dataUri);

    showConflictPage();

    ga('set', 'page', conflictSlug);
    ga('send', 'pageview');
    
    history.pushState(null, null, "index.html#" + conflictSlug);
}

function buildConflictPage(data) {
    var name = "No conflict name - ethics data for conflict not found";
    var description = "No conflict description - ethics data for conflict not found";

    if (data != null) {
        name = data.name;
        description = data.description;
    }

/*     var conflict = d3.select('#conflict');
    conflict.text(name); */

    var conflictDiv = d3.select('#conflict-title');
    conflictDiv.html(familyMemberPhotos(data) + '<span class="conflict-page-title">' + name + '</span>'); 

    var conflictDescription = d3.select('#conflictDescription');
    conflictDescription.text(description);

    addStories(data);
    addEthics(data);
}


function familyMemberPhotos(d) {
    let images = "";
    d.familyMembers.forEach(function (familyMember) {
        images = images + '<img class="tiny-photo" src="img/' + familyMember + '.jpg" alt="' + familyMember + '" height="64">';
    });
    return images;
}

function addStories(data) {
    if (data.stories.length == 0) {
        var html = h2("Media Accounts");
        html += "<hr>";
        html += info("No media accounts found");

        d3.select('#stories').html(html);
        return
    }

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

    var html = h2("Linked Businesses, Ownership Structure and Family Member Interests");
    html += info("Below find all business entities associated with this potential conflict, along with each entity’s ownership structure as reported on public financial disclosures. The ownership structure, as detailed on public financial disclosures, forms the basis for linking business entities.");
    html += "<hr>"; 

    var cols = [
        {"header": "Owned by",      "class": "td-text",    "field": "owner"}, 
        {"header": "Percentage (if known)", "class": "td-percent", "field": "percentage"}
    ];

    let ethics = data.ethics[0];
    if (ethics != null)
        ethics.familyMemberEthics.forEach(bus => {
            html += h4(bus.business + " / " + bus.familyMember + " / " + bus.conflictStatus);
            html += p(bus.description);
            html += makeTable(cols, bus.ownerships);
        });
    d3.select('#ethics').html(html);
}


function showConflictPage() {
    
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
