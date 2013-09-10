
//print(window);
//print(window.jQuery);

//for (var k in window) {
//    print(k);
//}


//var div = window.jQuery("div");
//div.each(function (i, item) {
//    var element = $(item);
//    //alert(element.text());
//    var value = element.text() + ' changed by jquery';
//    //alert(value);
//    element.text(value);
//});

//alert(div.text());
//div.html('dada');

//$("h1").html('There is a dynamic title !');
//$("h1").hide();

alert(bag.toto.a);

(function () {
    alert('typeof window.google:' + typeof window.google);
    window.google = {
        kEI: "o1EvUv7qMcq60wX484HgAg", getEI: function (a) { for (var b; a && (!a.getAttribute || !(b = a.getAttribute("eid"))) ;) a = a.parentNode; return b || google.kEI }, https: function () { return "https:" == window.location.protocol }, kEXPI: "17259,18168,4000116,4004949,4004953,4006727,4007080,4007232,4007277,4007661,4007713,4007829,4008067,4008133,4008142,4008234,4008488,4008567,4008725,4008821,4009002,4009004,4009021,4009057,4009220,4009352,4009386,4009396,4009397,4009399,4009565,4009822,4009873,4009923,4010015,4010057,4010066,4010084,4010131,4010144,4010161,4010286,4010308,4010530", kCSI: { e: "17259,18168,4000116,4004949,4004953,4006727,4007080,4007232,4007277,4007661,4007713,4007829,4008067,4008133,4008142,4008234,4008488,4008567,4008725,4008821,4009002,4009004,4009021,4009057,4009220,4009352,4009386,4009396,4009397,4009399,4009565,4009822,4009873,4009923,4010015,4010057,4010066,4010084,4010131,4010144,4010161,4010286,4010308,4010530", ei: "o1EvUv7qMcq60wX484HgAg" }, authuser: 0, ml: function () { }, kHL: "fr", time: function () { return (new Date).getTime() }, log: function (a, b, c, l, k) {
            var d = new Image, f = google.lc, e = google.li, g = "", h = "gen_204"; k && (h =
            k); d.onerror = d.onload = d.onabort = function () { delete f[e] }; f[e] = d; c || -1 != b.search("&ei=") || (g = "&ei=" + google.getEI(l)); c = c || "/" + h + "?atyp=i&ct=" + a + "&cad=" + b + g + "&zx=" + google.time();
            a = /^http:/i; a.test(c) && google.https() ? (google.ml(Error("GLMM"), !1, { src: c }), delete f[e]) : (d.src = c, google.li = e + 1)
        }, lc: [], li: 0, j: { en: 1, b: !!location.hash && !!location.hash.match("[#&]((q|fp)=|tbs=simg|tbs=sbi)"), bv: 20, pm: "", u: "c9c918f0" }, Toolbelt: {}, y: {}, x: function (a,
        b) { google.y[a.id] = [a, b]; return !1 }, load: function (a, b, c) { google.x({ id: a + m++ }, function () { google.load(a, b, c) }) }
    };
    alert(typeof window.google);
    var m = 0; window.onpopstate = function () { google.j.psc = 1 };
})();
(function () {
    google.sn = "webhp"; google.timers = {};
    google.startTick = function (a, b) { google.timers[a] = { t: { start: google.time() }, bfr: !!b } };

    alert('typeof window.google:' + typeof window.google);

    for (var kg in google) {
        alert(kg + ': ' + google[kg]);
    }

    google.tick = function (a, b, g) { google.timers[a] || google.startTick(a); google.timers[a].t[b] = g || google.time() };
    google.startTick("load", !0);
    try { google.pt = window.chrome && window.chrome.csi && Math.floor(window.chrome.csi().pageT); } catch (d) { }
})();
(function () {
    'use strict'; var c = this, f = Date.now || function () { return +new Date }; var m = function (d, k) { return function (a) { a || (a = window.event); return k.call(d, a) } }, t = "undefined" != typeof navigator && /Macintosh/.test(navigator.userAgent), u = "undefined" != typeof navigator && !/Opera/.test(navigator.userAgent) && /WebKit/.test(navigator.userAgent), v = "undefined" != typeof navigator && !/Opera|WebKit/.test(navigator.userAgent) && /Gecko/.test(navigator.product), x = v ? "keypress" : "keydown"; var y = function () { this.g = []; this.a = []; this.e = {}; this.d = null; this.c = [] }, z = "undefined" != typeof navigator && /iPhone|iPad|iPod/.test(navigator.userAgent), A = /\s*;\s*/, B = function (d, k) {
        return function (a) {
            var b; e: {
                b = k; if ("click" == b && (t && a.metaKey || !t && a.ctrlKey || 2 == a.which || null == a.which && 4 == a.button || a.shiftKey)) b = "clickmod"; else {
                    var e = a.which || a.keyCode || a.key, g; if (g = a.type == x) { g = a.srcElement || a.target; var n = g.tagName.toUpperCase(); g = !("TEXTAREA" == n || "BUTTON" == n || "INPUT" == n || "A" == n || g.isContentEditable) && !(a.ctrlKey || a.shiftKey || a.altKey || a.metaKey) && (13 == e || 32 == e || u && 3 == e) } g &&
                    (b = "clickkey")
                } for (g = e = a.srcElement || a.target; g && g != this; g = g.parentNode) { var n = g, l; var h = n; l = b; var p = h.__jsaction; if (!p) { p = {}; h.__jsaction = p; var r = null; "getAttribute" in h && (r = h.getAttribute("jsaction")); if (h = r) for (var h = h.split(A), r = 0, P = h ? h.length : 0; r < P; r++) { var q = h[r]; if (q) { var w = q.indexOf(":"), H = -1 != w, Q = H ? q.substr(0, w).replace(/^\s+/, "").replace(/\s+$/, "") : "click", q = H ? q.substr(w + 1).replace(/^\s+/, "").replace(/\s+$/, "") : q; p[Q] = q } } } h = void 0; "clickkey" == l ? l = "click" : "click" == l && (h = p.click || p.clickonly); l = (h = h || p[l]) ? { h: l, action: h } : void 0; if (l) { b = { eventType: l.h, event: a, targetElement: e, action: l.action, actionElement: n }; break e } } b = null
            } if (b) if ("A" == b.actionElement.tagName && "click" == k && (a.preventDefault ? a.preventDefault() : a.returnValue = !1), d.d) d.d(b); else { var s; if ((e = c.document) && !e.createEvent && e.createEventObject) try { s = e.createEventObject(a) } catch (U) { s = a } else s = a; v && (s.timeStamp = f()); b.event = s; d.c.push(b) }
        }
    }, C = function (d, k) { return function (a) { var b = d, e = k, g = !1; "mouseenter" == b ? b = "mouseover" : "mouseleave" == b && (b = "mouseout"); if (a.addEventListener) { if ("focus" == b || "blur" == b) g = !0; a.addEventListener(b, e, g) } else a.attachEvent && ("focus" == b ? b = "focusin" : "blur" == b && (b = "focusout"), e = m(a, e), a.attachEvent("on" + b, e)); return { h: b, i: e, capture: g } } }, D = function (d, k) { if (!d.e.hasOwnProperty(k) && "mouseenter" != k && "mouseleave" != k) { var a = B(d, k), b = C(k, a); d.e[k] = a; d.g.push(b); for (a = 0; a < d.a.length; ++a) { var e = d.a[a]; e.c.push(b.call(null, e.a)) } "click" == k && D(d, x) } }; y.prototype.i = function (d) { return this.e[d] }; var F = function () { this.a = E; this.c = [] }; var G = new y, E = window.document.documentElement, I; e: { for (var J = 0; J < G.a.length; J++) { for (var K = G.a[J].a, L = E; K != L && L.parentNode;) L = L.parentNode; if (K == L) { I = !0; break e } } I = !1 } if (!I) { z && (E.style.cursor = "pointer"); for (var M = new F, N = 0; N < G.g.length; ++N) M.c.push(G.g[N].call(null, M.a)); G.a.push(M) } D(G, "click"); D(G, "focus"); D(G, "focusin"); D(G, "blur"); D(G, "focusout"); D(G, "change"); D(G, "input"); D(G, "keydown"); D(G, "keypress"); D(G, "mousedown"); D(G, "mouseout"); D(G, "mouseover"); D(G, "mouseup"); D(G, "touchstart"); D(G, "touchmove"); D(G, "touchend"); D(G, "speech"); var O = function (d) { G.d = d; G.c && (0 < G.c.length && d(G.c), G.c = null) }, R = ["google", "jsad"], S = c; R[0] in S || !S.execScript || S.execScript("var " + R[0]); for (var T; R.length && (T = R.shift()) ;) R.length || void 0 === O ? S = S[T] ? S[T] : S[T] = {} : S[T] = O;
}).call(window); google.arwt = function (a) { a.href = document.getElementById(a.id.substring(1)).href; return !0 };

