

print('Nous sommes le ' + new Date());

for (var i = 0; i < 10; i++) {
    print("test " + i);
}
 

var chaine = "Les chiens et les chiennes, les chats et les oiseaux";
//var reg = new RegExp("(chien)", "g");
var reg = /(chien)/g;
print("Chaîne d'origine : " + chaine + "<BR>");
print("Chaîne traitée : " + chaine.replace(reg, "<SPAN style='background-color=yellow'>$1</SPAN></FONT>") + "<BR>");


//print(document);

//document.write("document works !");


////document.innerHTML = "html 1";
//print(document.innerHTML);

var e = window.createElement("div");

var div = document.createElement("div");

function parseHTML(data, context, keepScripts) {
    if (!data || typeof data !== "string") {
        return null;
    }
    if (typeof context === "boolean") {
        keepScripts = context;
        context = false;
    }
    context = context || document;

    var parsed = rsingleTag.exec(data),
        scripts = !keepScripts && [];

    // Single tag
    if (parsed) {
        return [context.createElement(parsed[1])];
    }

    parsed = jQuery.buildFragment([data], context, scripts);
    if (scripts) {
        jQuery(scripts).remove();
    }
    return jQuery.merge([], parsed.childNodes);
}

window.jQuery = "dzdz";


var window2 = {
    title: "",
    url: "http://www.google.fr"
};


window2.jQuery = "dzdz";

