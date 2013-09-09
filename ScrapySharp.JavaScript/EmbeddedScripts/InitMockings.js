
var document = new Document();
var window = new Window(document);

window.document = document;


function __InitWindow(w) {
    window = w;
    document = window.document;
}

function __LoadHtml(html) {
    document.LoadHtml(html);
}

//var window = {
//    title: "",
//    url: "http://www.google.fr",
    
//    W: null,
//    //createElement: W.CreateElement
//};



