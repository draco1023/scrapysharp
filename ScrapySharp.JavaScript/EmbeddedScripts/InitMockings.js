
var document = new Document();
var window = new Window(document);

window.document = document;

var bag = null;

function __InitBag(b) {
    bag = b;
    bag.dzdz = 'dz';
    bag.toto = {
        a: 'dede',
        sa: 54
    };
}

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



