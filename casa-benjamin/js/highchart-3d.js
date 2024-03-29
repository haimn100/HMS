﻿/*
 Highcharts JS v5.0.14 (2017-07-28)

 3D features for Highcharts JS

 @license: www.highcharts.com/license
*/
(function (y) { "object" === typeof module && module.exports ? module.exports = y : y(Highcharts) })(function (y) {
    (function (b) {
        var v = b.deg2rad, t = b.pick; b.perspective = function (u, w, x) {
            var k = w.options.chart.options3d, n = x ? w.inverted : !1, f = w.plotWidth / 2, p = w.plotHeight / 2, l = k.depth / 2, d = t(k.depth, 1) * t(k.viewDistance, 0), a = w.scale3d || 1, c = v * k.beta * (n ? -1 : 1), k = v * k.alpha * (n ? -1 : 1), e = Math.cos(k), g = Math.cos(-c), h = Math.sin(k), A = Math.sin(-c); x || (f += w.plotLeft, p += w.plotTop); return b.map(u, function (c) {
                var b, r; r = (n ? c.y : c.x) - f; var m = (n ?
                    c.x : c.y) - p, G = (c.z || 0) - l; b = g * r - A * G; c = -h * A * r + e * m - g * h * G; r = e * A * r + h * m + e * g * G; m = 0 < d && d < Number.POSITIVE_INFINITY ? d / (r + l + d) : 1; b = b * m * a + f; c = c * m * a + p; return { x: n ? c : b, y: n ? b : c, z: r * a + l }
            })
        }; b.pointCameraDistance = function (b, w) { var u = w.options.chart.options3d, k = w.plotWidth / 2; w = w.plotHeight / 2; u = t(u.depth, 1) * t(u.viewDistance, 0) + u.depth; return Math.sqrt(Math.pow(k - b.plotX, 2) + Math.pow(w - b.plotY, 2) + Math.pow(u - b.plotZ, 2)) }; b.shapeArea = function (b) {
            var u = 0, t, k; for (t = 0; t < b.length; t++)k = (t + 1) % b.length, u += b[t].x * b[k].y - b[k].x *
                b[t].y; return u / 2
        }; b.shapeArea3d = function (t, w, v) { return b.shapeArea(b.perspective(t, w, v)) }
    })(y); (function (b) {
        function v(a, e, c, g, d, b, h, l) {
            var B = [], m = b - d; return b > d && b - d > Math.PI / 2 + .0001 ? (B = B.concat(v(a, e, c, g, d, d + Math.PI / 2, h, l)), B = B.concat(v(a, e, c, g, d + Math.PI / 2, b, h, l))) : b < d && d - b > Math.PI / 2 + .0001 ? (B = B.concat(v(a, e, c, g, d, d - Math.PI / 2, h, l)), B = B.concat(v(a, e, c, g, d - Math.PI / 2, b, h, l))) : ["C", a + c * Math.cos(d) - c * r * m * Math.sin(d) + h, e + g * Math.sin(d) + g * r * m * Math.cos(d) + l, a + c * Math.cos(b) + c * r * m * Math.sin(b) + h, e + g * Math.sin(b) -
                g * r * m * Math.cos(b) + l, a + c * Math.cos(b) + h, e + g * Math.sin(b) + l]
        } var t = Math.cos, u = Math.PI, w = Math.sin, x = b.animObject, k = b.charts, n = b.color, f = b.defined, p = b.deg2rad, l = b.each, d = b.extend, a = b.inArray, c = b.map, e = b.merge, g = b.perspective, h = b.pick, A = b.SVGElement, q = b.SVGRenderer, z = b.wrap, r = 4 * (Math.sqrt(2) - 1) / 3 / (u / 2); q.prototype.toLinePath = function (a, e) { var c = []; l(a, function (a) { c.push("L", a.x, a.y) }); a.length && (c[0] = "M", e && c.push("Z")); return c }; q.prototype.toLineSegments = function (a) {
            var e = [], c = !0; l(a, function (a) {
                e.push(c ?
                    "M" : "L", a.x, a.y); c = !c
            }); return e
        }; q.prototype.face3d = function (a) {
            var e = this, c = this.createElement("path"); c.vertexes = []; c.insidePlotArea = !1; c.enabled = !0; z(c, "attr", function (a, c) {
                if ("object" === typeof c && (f(c.enabled) || f(c.vertexes) || f(c.insidePlotArea))) {
                this.enabled = h(c.enabled, this.enabled); this.vertexes = h(c.vertexes, this.vertexes); this.insidePlotArea = h(c.insidePlotArea, this.insidePlotArea); delete c.enabled; delete c.vertexes; delete c.insidePlotArea; var d = g(this.vertexes, k[e.chartIndex], this.insidePlotArea),
                    B = e.toLinePath(d, !0), d = b.shapeArea(d), d = this.enabled && 0 < d ? "visible" : "hidden"; c.d = B; c.visibility = d
                } return a.apply(this, [].slice.call(arguments, 1))
            }); z(c, "animate", function (a, c) {
                if ("object" === typeof c && (f(c.enabled) || f(c.vertexes) || f(c.insidePlotArea))) {
                this.enabled = h(c.enabled, this.enabled); this.vertexes = h(c.vertexes, this.vertexes); this.insidePlotArea = h(c.insidePlotArea, this.insidePlotArea); delete c.enabled; delete c.vertexes; delete c.insidePlotArea; var d = g(this.vertexes, k[e.chartIndex], this.insidePlotArea),
                    B = e.toLinePath(d, !0), d = b.shapeArea(d), d = this.enabled && 0 < d ? "visible" : "hidden"; c.d = B; this.attr("visibility", d)
                } return a.apply(this, [].slice.call(arguments, 1))
            }); return c.attr(a)
        }; q.prototype.polyhedron = function (a) {
            var c = this, e = this.g(), d = e.destroy; e.attr({ "stroke-linejoin": "round" }); e.faces = []; e.destroy = function () { for (var a = 0; a < e.faces.length; a++)e.faces[a].destroy(); return d.call(this) }; z(e, "attr", function (a, d, g, b, h) {
                if ("object" === typeof d && f(d.faces)) {
                    for (; e.faces.length > d.faces.length;)e.faces.pop().destroy();
                    for (; e.faces.length < d.faces.length;)e.faces.push(c.face3d().add(e)); for (var m = 0; m < d.faces.length; m++)e.faces[m].attr(d.faces[m], null, b, h); delete d.faces
                } return a.apply(this, [].slice.call(arguments, 1))
            }); z(e, "animate", function (a, d, g, b) {
                if (d && d.faces) { for (; e.faces.length > d.faces.length;)e.faces.pop().destroy(); for (; e.faces.length < d.faces.length;)e.faces.push(c.face3d().add(e)); for (var h = 0; h < d.faces.length; h++)e.faces[h].animate(d.faces[h], g, b); delete d.faces } return a.apply(this, [].slice.call(arguments,
                    1))
            }); return e.attr(a)
        }; q.prototype.cuboid = function (a) {
            var c = this.g(), e = c.destroy; a = this.cuboidPath(a); c.attr({ "stroke-linejoin": "round" }); c.front = this.path(a[0]).attr({ "class": "highcharts-3d-front" }).add(c); c.top = this.path(a[1]).attr({ "class": "highcharts-3d-top" }).add(c); c.side = this.path(a[2]).attr({ "class": "highcharts-3d-side" }).add(c); c.fillSetter = function (a) {
                this.front.attr({ fill: a }); this.top.attr({ fill: n(a).brighten(.1).get() }); this.side.attr({ fill: n(a).brighten(-.1).get() }); this.color = a; c.fill =
                    a; return this
            }; c.opacitySetter = function (a) { this.front.attr({ opacity: a }); this.top.attr({ opacity: a }); this.side.attr({ opacity: a }); return this }; c.attr = function (a, c, e, d) { if ("string" === typeof a && "undefined" !== typeof c) { var g = a; a = {}; a[g] = c } if (a.shapeArgs || f(a.x)) a = this.renderer.cuboidPath(a.shapeArgs || a), this.front.attr({ d: a[0] }), this.top.attr({ d: a[1] }), this.side.attr({ d: a[2] }); else return A.prototype.attr.call(this, a, void 0, e, d); return this }; c.animate = function (a, c, e) {
            f(a.x) && f(a.y) ? (a = this.renderer.cuboidPath(a),
                this.front.animate({ d: a[0] }, c, e), this.top.animate({ d: a[1] }, c, e), this.side.animate({ d: a[2] }, c, e), this.attr({ zIndex: -a[3] })) : a.opacity ? (this.front.animate(a, c, e), this.top.animate(a, c, e), this.side.animate(a, c, e)) : A.prototype.animate.call(this, a, c, e); return this
            }; c.destroy = function () { this.front.destroy(); this.top.destroy(); this.side.destroy(); return e.call(this) }; c.attr({ zIndex: -a[3] }); return c
        }; b.SVGRenderer.prototype.cuboidPath = function (a) {
            function e(a) { return t[a] } var d = a.x, h = a.y, m = a.z, l = a.height,
                p = a.width, r = a.depth, A = k[this.chartIndex], q, z, n = A.options.chart.options3d.alpha, f = 0, t = [{ x: d, y: h, z: m }, { x: d + p, y: h, z: m }, { x: d + p, y: h + l, z: m }, { x: d, y: h + l, z: m }, { x: d, y: h + l, z: m + r }, { x: d + p, y: h + l, z: m + r }, { x: d + p, y: h, z: m + r }, { x: d, y: h, z: m + r }], t = g(t, A, a.insidePlotArea); z = function (a, d) { var g = [[], -1]; a = c(a, e); d = c(d, e); 0 > b.shapeArea(a) ? g = [a, 0] : 0 > b.shapeArea(d) && (g = [d, 1]); return g }; q = z([3, 2, 1, 0], [7, 6, 5, 4]); a = q[0]; p = q[1]; q = z([1, 6, 7, 0], [4, 5, 2, 3]); l = q[0]; r = q[1]; q = z([1, 2, 5, 6], [0, 7, 4, 3]); z = q[0]; q = q[1]; 1 === q ? f += 1E4 * (1E3 - d) : q ||
                    (f += 1E4 * d); f += 10 * (!r || 0 <= n && 180 >= n || 360 > n && 357.5 < n ? A.plotHeight - h : 10 + h); 1 === p ? f += 100 * m : p || (f += 100 * (1E3 - m)); f = -Math.round(f); return [this.toLinePath(a, !0), this.toLinePath(l, !0), this.toLinePath(z, !0), f]
        }; b.SVGRenderer.prototype.arc3d = function (c) {
            function g(c) { var d = !1, b = {}; c = e(c); for (var g in c) -1 !== a(g, q) && (b[g] = c[g], delete c[g], d = !0); return d ? b : !1 } var b = this.g(), m = b.renderer, q = "x y r innerR start end".split(" "); c = e(c); c.alpha *= p; c.beta *= p; b.top = m.path(); b.side1 = m.path(); b.side2 = m.path(); b.inn = m.path();
            b.out = m.path(); b.onAdd = function () { var a = b.parentGroup, c = b.attr("class"); b.top.add(b); l(["out", "inn", "side1", "side2"], function (e) { b[e].attr({ "class": c + " highcharts-3d-side" }).add(a) }) }; l(["addClass", "removeClass"], function (a) { b[a] = function () { var c = arguments; l(["top", "out", "inn", "side1", "side2"], function (e) { b[e][a].apply(b[e], c) }) } }); b.setPaths = function (a) {
                var c = b.renderer.arc3dPath(a), e = 100 * c.zTop; b.attribs = a; b.top.attr({ d: c.top, zIndex: c.zTop }); b.inn.attr({ d: c.inn, zIndex: c.zInn }); b.out.attr({
                    d: c.out,
                    zIndex: c.zOut
                }); b.side1.attr({ d: c.side1, zIndex: c.zSide1 }); b.side2.attr({ d: c.side2, zIndex: c.zSide2 }); b.zIndex = e; b.attr({ zIndex: e }); a.center && (b.top.setRadialReference(a.center), delete a.center)
            }; b.setPaths(c); b.fillSetter = function (a) { var c = n(a).brighten(-.1).get(); this.fill = a; this.side1.attr({ fill: c }); this.side2.attr({ fill: c }); this.inn.attr({ fill: c }); this.out.attr({ fill: c }); this.top.attr({ fill: a }); return this }; l(["opacity", "translateX", "translateY", "visibility"], function (a) {
            b[a + "Setter"] = function (a,
                c) { b[c] = a; l(["out", "inn", "side1", "side2", "top"], function (e) { b[e].attr(c, a) }) }
            }); z(b, "attr", function (a, c) { var e; "object" === typeof c && (e = g(c)) && (d(b.attribs, e), b.setPaths(b.attribs)); return a.apply(this, [].slice.call(arguments, 1)) }); z(b, "animate", function (a, c, d, b) {
                var m, l = this.attribs, p; delete c.center; delete c.z; delete c.depth; delete c.alpha; delete c.beta; p = x(h(d, this.renderer.globalAnimation)); p.duration && (m = g(c), c.dummy = 1, m && (p.step = function (a, c) {
                    function d(a) { return l[a] + (h(m[a], l[a]) - l[a]) * c.pos }
                    "dummy" === c.prop && c.elem.setPaths(e(l, { x: d("x"), y: d("y"), r: d("r"), innerR: d("innerR"), start: d("start"), end: d("end") }))
                }), d = p); return a.call(this, c, d, b)
            }); b.destroy = function () { this.top.destroy(); this.out.destroy(); this.inn.destroy(); this.side1.destroy(); this.side2.destroy(); A.prototype.destroy.call(this) }; b.hide = function () { this.top.hide(); this.out.hide(); this.inn.hide(); this.side1.hide(); this.side2.hide() }; b.show = function () { this.top.show(); this.out.show(); this.inn.show(); this.side1.show(); this.side2.show() };
            return b
        }; q.prototype.arc3dPath = function (a) {
            function c(a) { a %= 2 * Math.PI; a > Math.PI && (a = 2 * Math.PI - a); return a } var e = a.x, d = a.y, b = a.start, g = a.end - .00001, h = a.r, l = a.innerR, m = a.depth, p = a.alpha, q = a.beta, r = Math.cos(b), A = Math.sin(b); a = Math.cos(g); var z = Math.sin(g), f = h * Math.cos(q), h = h * Math.cos(p), k = l * Math.cos(q), n = l * Math.cos(p), l = m * Math.sin(q), x = m * Math.sin(p), m = ["M", e + f * r, d + h * A], m = m.concat(v(e, d, f, h, b, g, 0, 0)), m = m.concat(["L", e + k * a, d + n * z]), m = m.concat(v(e, d, k, n, g, b, 0, 0)), m = m.concat(["Z"]), y = 0 < q ? Math.PI / 2 : 0, q = 0 <
                p ? 0 : Math.PI / 2, y = b > -y ? b : g > -y ? -y : b, C = g < u - q ? g : b < u - q ? u - q : g, D = 2 * u - q, p = ["M", e + f * t(y), d + h * w(y)], p = p.concat(v(e, d, f, h, y, C, 0, 0)); g > D && b < D ? (p = p.concat(["L", e + f * t(C) + l, d + h * w(C) + x]), p = p.concat(v(e, d, f, h, C, D, l, x)), p = p.concat(["L", e + f * t(D), d + h * w(D)]), p = p.concat(v(e, d, f, h, D, g, 0, 0)), p = p.concat(["L", e + f * t(g) + l, d + h * w(g) + x]), p = p.concat(v(e, d, f, h, g, D, l, x)), p = p.concat(["L", e + f * t(D), d + h * w(D)]), p = p.concat(v(e, d, f, h, D, C, 0, 0))) : g > u - q && b < u - q && (p = p.concat(["L", e + f * Math.cos(C) + l, d + h * Math.sin(C) + x]), p = p.concat(v(e, d, f, h, C, g,
                    l, x)), p = p.concat(["L", e + f * Math.cos(g), d + h * Math.sin(g)]), p = p.concat(v(e, d, f, h, g, C, 0, 0))); p = p.concat(["L", e + f * Math.cos(C) + l, d + h * Math.sin(C) + x]); p = p.concat(v(e, d, f, h, C, y, l, x)); p = p.concat(["Z"]); q = ["M", e + k * r, d + n * A]; q = q.concat(v(e, d, k, n, b, g, 0, 0)); q = q.concat(["L", e + k * Math.cos(g) + l, d + n * Math.sin(g) + x]); q = q.concat(v(e, d, k, n, g, b, l, x)); q = q.concat(["Z"]); r = ["M", e + f * r, d + h * A, "L", e + f * r + l, d + h * A + x, "L", e + k * r + l, d + n * A + x, "L", e + k * r, d + n * A, "Z"]; e = ["M", e + f * a, d + h * z, "L", e + f * a + l, d + h * z + x, "L", e + k * a + l, d + n * z + x, "L", e + k * a, d + n * z, "Z"];
            z = Math.atan2(x, -l); d = Math.abs(g + z); a = Math.abs(b + z); b = Math.abs((b + g) / 2 + z); d = c(d); a = c(a); b = c(b); b *= 1E5; g = 1E5 * a; d *= 1E5; return { top: m, zTop: 1E5 * Math.PI + 1, out: p, zOut: Math.max(b, g, d), inn: q, zInn: Math.max(b, g, d), side1: r, zSide1: .99 * d, side2: e, zSide2: .99 * g }
        }
    })(y); (function (b) {
        function v(b, l) {
            var d = b.plotLeft, a = b.plotWidth + d, c = b.plotTop, e = b.plotHeight + c, g = d + b.plotWidth / 2, h = c + b.plotHeight / 2, p = Number.MAX_VALUE, q = -Number.MAX_VALUE, f = Number.MAX_VALUE, r = -Number.MAX_VALUE, m, k = 1; m = [{ x: d, y: c, z: 0 }, { x: d, y: c, z: l }]; u([0, 1],
                function (c) { m.push({ x: a, y: m[c].y, z: m[c].z }) }); u([0, 1, 2, 3], function (a) { m.push({ x: m[a].x, y: e, z: m[a].z }) }); m = x(m, b, !1); u(m, function (a) { p = Math.min(p, a.x); q = Math.max(q, a.x); f = Math.min(f, a.y); r = Math.max(r, a.y) }); d > p && (k = Math.min(k, 1 - Math.abs((d + g) / (p + g)) % 1)); a < q && (k = Math.min(k, (a - g) / (q - g))); c > f && (k = 0 > f ? Math.min(k, (c + h) / (-f + c + h)) : Math.min(k, 1 - (c + h) / (f + h) % 1)); e < r && (k = Math.min(k, Math.abs((e - h) / (r - h)))); return k
        } var t = b.Chart, u = b.each, w = b.merge, x = b.perspective, k = b.pick, n = b.wrap; t.prototype.is3d = function () {
            return this.options.chart.options3d &&
                this.options.chart.options3d.enabled
        }; t.prototype.propsRequireDirtyBox.push("chart.options3d"); t.prototype.propsRequireUpdateSeries.push("chart.options3d"); b.wrap(b.Chart.prototype, "isInsidePlot", function (b) { return this.is3d() || b.apply(this, [].slice.call(arguments, 1)) }); var f = b.getOptions(); w(!0, f, { chart: { options3d: { enabled: !1, alpha: 0, beta: 0, depth: 100, fitToPlot: !0, viewDistance: 25, axisLabelPosition: "default", frame: { visible: "default", size: 1, bottom: {}, top: {}, left: {}, right: {}, back: {}, front: {} } } } }); n(t.prototype,
            "setClassName", function (b) { b.apply(this, [].slice.call(arguments, 1)); this.is3d() && (this.container.className += " highcharts-3d-chart") }); b.wrap(b.Chart.prototype, "setChartSize", function (b) {
                var l = this.options.chart.options3d; b.apply(this, [].slice.call(arguments, 1)); if (this.is3d()) {
                    var d = this.inverted, a = this.clipBox, c = this.margin; a[d ? "y" : "x"] = -(c[3] || 0); a[d ? "x" : "y"] = -(c[0] || 0); a[d ? "height" : "width"] = this.chartWidth + (c[3] || 0) + (c[1] || 0); a[d ? "width" : "height"] = this.chartHeight + (c[0] || 0) + (c[2] || 0); this.scale3d =
                        1; !0 === l.fitToPlot && (this.scale3d = v(this, l.depth))
                }
            }); n(t.prototype, "redraw", function (b) { this.is3d() && (this.isDirtyBox = !0, this.frame3d = this.get3dFrame()); b.apply(this, [].slice.call(arguments, 1)) }); n(t.prototype, "render", function (b) { this.is3d() && (this.frame3d = this.get3dFrame()); b.apply(this, [].slice.call(arguments, 1)) }); n(t.prototype, "renderSeries", function (b) { var l = this.series.length; if (this.is3d()) for (; l--;)b = this.series[l], b.translate(), b.render(); else b.call(this) }); n(t.prototype, "drawChartBox",
                function (p) {
                    if (this.is3d()) {
                        var l = this.renderer, d = this.options.chart.options3d, a = this.get3dFrame(), c = this.plotLeft, e = this.plotLeft + this.plotWidth, g = this.plotTop, h = this.plotTop + this.plotHeight, d = d.depth, f = c - (a.left.visible ? a.left.size : 0), q = e + (a.right.visible ? a.right.size : 0), k = g - (a.top.visible ? a.top.size : 0), r = h + (a.bottom.visible ? a.bottom.size : 0), m = 0 - (a.front.visible ? a.front.size : 0), n = d + (a.back.visible ? a.back.size : 0), t = this.hasRendered ? "animate" : "attr"; this.frame3d = a; this.frameShapes || (this.frameShapes =
                            { bottom: l.polyhedron().add(), top: l.polyhedron().add(), left: l.polyhedron().add(), right: l.polyhedron().add(), back: l.polyhedron().add(), front: l.polyhedron().add() }); this.frameShapes.bottom[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-bottom", zIndex: a.bottom.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.bottom.color).brighten(.1).get(), vertexes: [{ x: f, y: r, z: m }, { x: q, y: r, z: m }, { x: q, y: r, z: n }, { x: f, y: r, z: n }], enabled: a.bottom.visible }, {
                                    fill: b.color(a.bottom.color).brighten(.1).get(), vertexes: [{
                                        x: c, y: h,
                                        z: d
                                    }, { x: e, y: h, z: d }, { x: e, y: h, z: 0 }, { x: c, y: h, z: 0 }], enabled: a.bottom.visible
                                }, { fill: b.color(a.bottom.color).brighten(-.1).get(), vertexes: [{ x: f, y: r, z: m }, { x: f, y: r, z: n }, { x: c, y: h, z: d }, { x: c, y: h, z: 0 }], enabled: a.bottom.visible && !a.left.visible }, { fill: b.color(a.bottom.color).brighten(-.1).get(), vertexes: [{ x: q, y: r, z: n }, { x: q, y: r, z: m }, { x: e, y: h, z: 0 }, { x: e, y: h, z: d }], enabled: a.bottom.visible && !a.right.visible }, {
                                    fill: b.color(a.bottom.color).get(), vertexes: [{ x: q, y: r, z: m }, { x: f, y: r, z: m }, { x: c, y: h, z: 0 }, { x: e, y: h, z: 0 }], enabled: a.bottom.visible &&
                                        !a.front.visible
                                }, { fill: b.color(a.bottom.color).get(), vertexes: [{ x: f, y: r, z: n }, { x: q, y: r, z: n }, { x: e, y: h, z: d }, { x: c, y: h, z: d }], enabled: a.bottom.visible && !a.back.visible }]
                            }); this.frameShapes.top[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-top", zIndex: a.top.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.top.color).brighten(.1).get(), vertexes: [{ x: f, y: k, z: n }, { x: q, y: k, z: n }, { x: q, y: k, z: m }, { x: f, y: k, z: m }], enabled: a.top.visible }, {
                                    fill: b.color(a.top.color).brighten(.1).get(), vertexes: [{ x: c, y: g, z: 0 }, {
                                        x: e, y: g,
                                        z: 0
                                    }, { x: e, y: g, z: d }, { x: c, y: g, z: d }], enabled: a.top.visible
                                }, { fill: b.color(a.top.color).brighten(-.1).get(), vertexes: [{ x: f, y: k, z: n }, { x: f, y: k, z: m }, { x: c, y: g, z: 0 }, { x: c, y: g, z: d }], enabled: a.top.visible && !a.left.visible }, { fill: b.color(a.top.color).brighten(-.1).get(), vertexes: [{ x: q, y: k, z: m }, { x: q, y: k, z: n }, { x: e, y: g, z: d }, { x: e, y: g, z: 0 }], enabled: a.top.visible && !a.right.visible }, { fill: b.color(a.top.color).get(), vertexes: [{ x: f, y: k, z: m }, { x: q, y: k, z: m }, { x: e, y: g, z: 0 }, { x: c, y: g, z: 0 }], enabled: a.top.visible && !a.front.visible },
                                { fill: b.color(a.top.color).get(), vertexes: [{ x: q, y: k, z: n }, { x: f, y: k, z: n }, { x: c, y: g, z: d }, { x: e, y: g, z: d }], enabled: a.top.visible && !a.back.visible }]
                            }); this.frameShapes.left[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-left", zIndex: a.left.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.left.color).brighten(.1).get(), vertexes: [{ x: f, y: r, z: m }, { x: c, y: h, z: 0 }, { x: c, y: h, z: d }, { x: f, y: r, z: n }], enabled: a.left.visible && !a.bottom.visible }, {
                                    fill: b.color(a.left.color).brighten(.1).get(), vertexes: [{ x: f, y: k, z: n }, {
                                        x: c, y: g,
                                        z: d
                                    }, { x: c, y: g, z: 0 }, { x: f, y: k, z: m }], enabled: a.left.visible && !a.top.visible
                                }, { fill: b.color(a.left.color).brighten(-.1).get(), vertexes: [{ x: f, y: r, z: n }, { x: f, y: k, z: n }, { x: f, y: k, z: m }, { x: f, y: r, z: m }], enabled: a.left.visible }, { fill: b.color(a.left.color).brighten(-.1).get(), vertexes: [{ x: c, y: g, z: d }, { x: c, y: h, z: d }, { x: c, y: h, z: 0 }, { x: c, y: g, z: 0 }], enabled: a.left.visible }, { fill: b.color(a.left.color).get(), vertexes: [{ x: f, y: r, z: m }, { x: f, y: k, z: m }, { x: c, y: g, z: 0 }, { x: c, y: h, z: 0 }], enabled: a.left.visible && !a.front.visible }, {
                                    fill: b.color(a.left.color).get(),
                                    vertexes: [{ x: f, y: k, z: n }, { x: f, y: r, z: n }, { x: c, y: h, z: d }, { x: c, y: g, z: d }], enabled: a.left.visible && !a.back.visible
                                }]
                            }); this.frameShapes.right[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-right", zIndex: a.right.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.right.color).brighten(.1).get(), vertexes: [{ x: q, y: r, z: n }, { x: e, y: h, z: d }, { x: e, y: h, z: 0 }, { x: q, y: r, z: m }], enabled: a.right.visible && !a.bottom.visible }, {
                                    fill: b.color(a.right.color).brighten(.1).get(), vertexes: [{ x: q, y: k, z: m }, { x: e, y: g, z: 0 }, { x: e, y: g, z: d }, {
                                        x: q,
                                        y: k, z: n
                                    }], enabled: a.right.visible && !a.top.visible
                                }, { fill: b.color(a.right.color).brighten(-.1).get(), vertexes: [{ x: e, y: g, z: 0 }, { x: e, y: h, z: 0 }, { x: e, y: h, z: d }, { x: e, y: g, z: d }], enabled: a.right.visible }, { fill: b.color(a.right.color).brighten(-.1).get(), vertexes: [{ x: q, y: r, z: m }, { x: q, y: k, z: m }, { x: q, y: k, z: n }, { x: q, y: r, z: n }], enabled: a.right.visible }, { fill: b.color(a.right.color).get(), vertexes: [{ x: q, y: k, z: m }, { x: q, y: r, z: m }, { x: e, y: h, z: 0 }, { x: e, y: g, z: 0 }], enabled: a.right.visible && !a.front.visible }, {
                                    fill: b.color(a.right.color).get(),
                                    vertexes: [{ x: q, y: r, z: n }, { x: q, y: k, z: n }, { x: e, y: g, z: d }, { x: e, y: h, z: d }], enabled: a.right.visible && !a.back.visible
                                }]
                            }); this.frameShapes.back[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-back", zIndex: a.back.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.back.color).brighten(.1).get(), vertexes: [{ x: q, y: r, z: n }, { x: f, y: r, z: n }, { x: c, y: h, z: d }, { x: e, y: h, z: d }], enabled: a.back.visible && !a.bottom.visible }, {
                                    fill: b.color(a.back.color).brighten(.1).get(), vertexes: [{ x: f, y: k, z: n }, { x: q, y: k, z: n }, { x: e, y: g, z: d }, { x: c, y: g, z: d }],
                                    enabled: a.back.visible && !a.top.visible
                                }, { fill: b.color(a.back.color).brighten(-.1).get(), vertexes: [{ x: f, y: r, z: n }, { x: f, y: k, z: n }, { x: c, y: g, z: d }, { x: c, y: h, z: d }], enabled: a.back.visible && !a.left.visible }, { fill: b.color(a.back.color).brighten(-.1).get(), vertexes: [{ x: q, y: k, z: n }, { x: q, y: r, z: n }, { x: e, y: h, z: d }, { x: e, y: g, z: d }], enabled: a.back.visible && !a.right.visible }, { fill: b.color(a.back.color).get(), vertexes: [{ x: c, y: g, z: d }, { x: e, y: g, z: d }, { x: e, y: h, z: d }, { x: c, y: h, z: d }], enabled: a.back.visible }, {
                                    fill: b.color(a.back.color).get(),
                                    vertexes: [{ x: f, y: r, z: n }, { x: q, y: r, z: n }, { x: q, y: k, z: n }, { x: f, y: k, z: n }], enabled: a.back.visible
                                }]
                            }); this.frameShapes.front[t]({
                                "class": "highcharts-3d-frame highcharts-3d-frame-front", zIndex: a.front.frontFacing ? -1E3 : 1E3, faces: [{ fill: b.color(a.front.color).brighten(.1).get(), vertexes: [{ x: f, y: r, z: m }, { x: q, y: r, z: m }, { x: e, y: h, z: 0 }, { x: c, y: h, z: 0 }], enabled: a.front.visible && !a.bottom.visible }, {
                                    fill: b.color(a.front.color).brighten(.1).get(), vertexes: [{ x: q, y: k, z: m }, { x: f, y: k, z: m }, { x: c, y: g, z: 0 }, { x: e, y: g, z: 0 }], enabled: a.front.visible &&
                                        !a.top.visible
                                }, { fill: b.color(a.front.color).brighten(-.1).get(), vertexes: [{ x: f, y: k, z: m }, { x: f, y: r, z: m }, { x: c, y: h, z: 0 }, { x: c, y: g, z: 0 }], enabled: a.front.visible && !a.left.visible }, { fill: b.color(a.front.color).brighten(-.1).get(), vertexes: [{ x: q, y: r, z: m }, { x: q, y: k, z: m }, { x: e, y: g, z: 0 }, { x: e, y: h, z: 0 }], enabled: a.front.visible && !a.right.visible }, { fill: b.color(a.front.color).get(), vertexes: [{ x: e, y: g, z: 0 }, { x: c, y: g, z: 0 }, { x: c, y: h, z: 0 }, { x: e, y: h, z: 0 }], enabled: a.front.visible }, {
                                    fill: b.color(a.front.color).get(), vertexes: [{
                                        x: q,
                                        y: r, z: m
                                    }, { x: f, y: r, z: m }, { x: f, y: k, z: m }, { x: q, y: k, z: m }], enabled: a.front.visible
                                }]
                            })
                    } return p.apply(this, [].slice.call(arguments, 1))
                }); t.prototype.retrieveStacks = function (b) { var f = this.series, d = {}, a, c = 1; u(this.series, function (e) { a = k(e.options.stack, b ? 0 : f.length - 1 - e.index); d[a] ? d[a].series.push(e) : (d[a] = { series: [e], position: c }, c++) }); d.totalStacks = c + 1; return d }; t.prototype.get3dFrame = function () {
                    var f = this, l = f.options.chart.options3d, d = l.frame, a = f.plotLeft, c = f.plotLeft + f.plotWidth, e = f.plotTop, g = f.plotTop +
                        f.plotHeight, h = l.depth, n = b.shapeArea3d([{ x: a, y: g, z: h }, { x: c, y: g, z: h }, { x: c, y: g, z: 0 }, { x: a, y: g, z: 0 }], f), q = b.shapeArea3d([{ x: a, y: e, z: 0 }, { x: c, y: e, z: 0 }, { x: c, y: e, z: h }, { x: a, y: e, z: h }], f), t = b.shapeArea3d([{ x: a, y: e, z: 0 }, { x: a, y: e, z: h }, { x: a, y: g, z: h }, { x: a, y: g, z: 0 }], f), r = b.shapeArea3d([{ x: c, y: e, z: h }, { x: c, y: e, z: 0 }, { x: c, y: g, z: 0 }, { x: c, y: g, z: h }], f), m = b.shapeArea3d([{ x: a, y: g, z: 0 }, { x: c, y: g, z: 0 }, { x: c, y: e, z: 0 }, { x: a, y: e, z: 0 }], f), w = b.shapeArea3d([{ x: a, y: e, z: h }, { x: c, y: e, z: h }, { x: c, y: g, z: h }, { x: a, y: g, z: h }], f), v = !1, y = !1, F = !1, H =
                        !1; u([].concat(f.xAxis, f.yAxis, f.zAxis), function (a) { a && (a.horiz ? a.opposite ? y = !0 : v = !0 : a.opposite ? H = !0 : F = !0) }); var E = function (a, c, e) { for (var d = ["size", "color", "visible"], b = {}, g = 0; g < d.length; g++)for (var h = d[g], f = 0; f < a.length; f++)if ("object" === typeof a[f]) { var l = a[f][h]; if (void 0 !== l && null !== l) { b[h] = l; break } } a = e; !0 === b.visible || !1 === b.visible ? a = b.visible : "auto" === b.visible && (a = 0 <= c); return { size: k(b.size, 1), color: k(b.color, "none"), frontFacing: 0 < c, visible: a } }, d = {
                            bottom: E([d.bottom, d.top, d], n, v), top: E([d.top,
                            d.bottom, d], q, y), left: E([d.left, d.right, d.side, d], t, F), right: E([d.right, d.left, d.side, d], r, H), back: E([d.back, d.front, d], w, !0), front: E([d.front, d.back, d], m, !1)
                        }; "auto" === l.axisLabelPosition ? (r = function (a, c) { return a.visible !== c.visible || a.visible && c.visible && a.frontFacing !== c.frontFacing }, l = [], r(d.left, d.front) && l.push({ y: (e + g) / 2, x: a, z: 0 }), r(d.left, d.back) && l.push({ y: (e + g) / 2, x: a, z: h }), r(d.right, d.front) && l.push({ y: (e + g) / 2, x: c, z: 0 }), r(d.right, d.back) && l.push({ y: (e + g) / 2, x: c, z: h }), n = [], r(d.bottom, d.front) &&
                            n.push({ x: (a + c) / 2, y: g, z: 0 }), r(d.bottom, d.back) && n.push({ x: (a + c) / 2, y: g, z: h }), q = [], r(d.top, d.front) && q.push({ x: (a + c) / 2, y: e, z: 0 }), r(d.top, d.back) && q.push({ x: (a + c) / 2, y: e, z: h }), t = [], r(d.bottom, d.left) && t.push({ z: (0 + h) / 2, y: g, x: a }), r(d.bottom, d.right) && t.push({ z: (0 + h) / 2, y: g, x: c }), g = [], r(d.top, d.left) && g.push({ z: (0 + h) / 2, y: e, x: a }), r(d.top, d.right) && g.push({ z: (0 + h) / 2, y: e, x: c }), a = function (a, c, e) {
                                if (0 === a.length) return null; if (1 === a.length) return a[0]; for (var d = 0, b = x(a, f, !1), g = 1; g < b.length; g++)e * b[g][c] > e * b[d][c] ?
                                    d = g : e * b[g][c] === e * b[d][c] && b[g].z < b[d].z && (d = g); return a[d]
                            }, d.axes = { y: { left: a(l, "x", -1), right: a(l, "x", 1) }, x: { top: a(q, "y", -1), bottom: a(n, "y", 1) }, z: { top: a(g, "y", -1), bottom: a(t, "y", 1) } }) : d.axes = { y: { left: { x: a, z: 0 }, right: { x: c, z: 0 } }, x: { top: { y: e, z: 0 }, bottom: { y: g, z: 0 } }, z: { top: { x: F ? c : a, y: e }, bottom: { x: F ? c : a, y: g } } }; return d
                }
    })(y); (function (b) {
        function v(a, e) {
            if (a.chart.is3d() && "colorAxis" !== a.coll) {
                var c = a.chart, d = c.frame3d, b = c.plotLeft, l = c.plotWidth + b, k = c.plotTop, c = c.plotHeight + k, n = 0, m = 0; e = a.swapZ({
                    x: e.x,
                    y: e.y, z: 0
                }); if (a.isZAxis) if (a.opposite) { if (null === d.axes.z.top) return {}; m = e.y - k; e.x = d.axes.z.top.x; e.y = d.axes.z.top.y } else { if (null === d.axes.z.bottom) return {}; m = e.y - c; e.x = d.axes.z.bottom.x; e.y = d.axes.z.bottom.y } else if (a.horiz) if (a.opposite) { if (null === d.axes.x.top) return {}; m = e.y - k; e.y = d.axes.x.top.y; e.z = d.axes.x.top.z } else { if (null === d.axes.x.bottom) return {}; m = e.y - c; e.y = d.axes.x.bottom.y; e.z = d.axes.x.bottom.z } else if (a.opposite) { if (null === d.axes.y.right) return {}; n = e.x - l; e.x = d.axes.y.right.x; e.z = d.axes.y.right.z } else {
                    if (null ===
                        d.axes.y.left) return {}; n = e.x - b; e.x = d.axes.y.left.x; e.z = d.axes.y.left.z
                } e = f([e], a.chart)[0]; e.x += n; e.y += m
            } return e
        } var t, u = b.Axis, w = b.Chart, x = b.each, k = b.extend, n = b.merge, f = b.perspective, p = b.pick, l = b.splat, d = b.Tick, a = b.wrap; a(u.prototype, "setOptions", function (a, d) { a.call(this, d); this.chart.is3d() && "colorAxis" !== this.coll && (a = this.options, a.tickWidth = p(a.tickWidth, 0), a.gridLineWidth = p(a.gridLineWidth, 1)) }); a(u.prototype, "getPlotLinePath", function (a) {
            var c = a.apply(this, [].slice.call(arguments, 1)); if (!this.chart.is3d() ||
                "colorAxis" === this.coll || null === c) return c; var d = this.chart, b = d.options.chart.options3d, b = this.isZAxis ? d.plotWidth : b.depth, d = d.frame3d, c = [this.swapZ({ x: c[1], y: c[2], z: 0 }), this.swapZ({ x: c[1], y: c[2], z: b }), this.swapZ({ x: c[4], y: c[5], z: 0 }), this.swapZ({ x: c[4], y: c[5], z: b })], b = []; this.horiz ? (this.isZAxis ? (d.left.visible && b.push(c[0], c[2]), d.right.visible && b.push(c[1], c[3])) : (d.front.visible && b.push(c[0], c[2]), d.back.visible && b.push(c[1], c[3])), d.top.visible && b.push(c[0], c[1]), d.bottom.visible && b.push(c[2],
                    c[3])) : (d.front.visible && b.push(c[0], c[2]), d.back.visible && b.push(c[1], c[3]), d.left.visible && b.push(c[0], c[1]), d.right.visible && b.push(c[2], c[3])); b = f(b, this.chart, !1); return this.chart.renderer.toLineSegments(b)
        }); a(u.prototype, "getLinePath", function (a) { return this.chart.is3d() ? [] : a.apply(this, [].slice.call(arguments, 1)) }); a(u.prototype, "getPlotBandPath", function (a) {
            if (!this.chart.is3d() || "colorAxis" === this.coll) return a.apply(this, [].slice.call(arguments, 1)); var c = arguments, d = c[2], b = [], c = this.getPlotLinePath(c[1]),
                d = this.getPlotLinePath(d); if (c && d) for (var f = 0; f < c.length; f += 6)b.push("M", c[f + 1], c[f + 2], "L", c[f + 4], c[f + 5], "L", d[f + 4], d[f + 5], "L", d[f + 1], d[f + 2], "Z"); return b
        }); a(d.prototype, "getMarkPath", function (a) { var c = a.apply(this, [].slice.call(arguments, 1)), c = [v(this.axis, { x: c[1], y: c[2], z: 0 }), v(this.axis, { x: c[4], y: c[5], z: 0 })]; return this.axis.chart.renderer.toLineSegments(c) }); a(d.prototype, "getLabelPosition", function (a) { var c = a.apply(this, [].slice.call(arguments, 1)); return v(this.axis, c) }); b.wrap(u.prototype,
            "getTitlePosition", function (a) { var c = a.apply(this, [].slice.call(arguments, 1)); return v(this, c) }); a(u.prototype, "drawCrosshair", function (a) { var c = arguments; this.chart.is3d() && c[2] && (c[2] = { plotX: c[2].plotXold || c[2].plotX, plotY: c[2].plotYold || c[2].plotY }); a.apply(this, [].slice.call(c, 1)) }); a(u.prototype, "destroy", function (a) { x(["backFrame", "bottomFrame", "sideFrame"], function (a) { this[a] && (this[a] = this[a].destroy()) }, this); a.apply(this, [].slice.call(arguments, 1)) }); u.prototype.swapZ = function (a, d) {
                return this.isZAxis ?
                    (d = d ? 0 : this.chart.plotLeft, { x: d + a.z, y: a.y, z: a.x - d }) : a
            }; t = b.ZAxis = function () { this.init.apply(this, arguments) }; k(t.prototype, u.prototype); k(t.prototype, {
                isZAxis: !0, setOptions: function (a) { a = n({ offset: 0, lineWidth: 0 }, a); u.prototype.setOptions.call(this, a); this.coll = "zAxis" }, setAxisSize: function () { u.prototype.setAxisSize.call(this); this.width = this.len = this.chart.options.chart.options3d.depth; this.right = this.chart.chartWidth - this.width - this.left }, getSeriesExtremes: function () {
                    var a = this, d = a.chart; a.hasVisibleSeries =
                        !1; a.dataMin = a.dataMax = a.ignoreMinPadding = a.ignoreMaxPadding = null; a.buildStacks && a.buildStacks(); x(a.series, function (c) { if (c.visible || !d.options.chart.ignoreHiddenSeries) a.hasVisibleSeries = !0, c = c.zData, c.length && (a.dataMin = Math.min(p(a.dataMin, c[0]), Math.min.apply(null, c)), a.dataMax = Math.max(p(a.dataMax, c[0]), Math.max.apply(null, c))) })
                }
            }); a(w.prototype, "getAxes", function (a) {
                var c = this, d = this.options, d = d.zAxis = l(d.zAxis || {}); a.call(this); c.is3d() && (this.zAxis = [], x(d, function (a, d) {
                a.index = d; a.isX = !0;
                    (new t(c, a)).setScale()
                }))
            })
    })(y); (function (b) {
        function v(b) { var d = b.apply(this, [].slice.call(arguments, 1)); this.chart.is3d() && (d.stroke = this.options.edgeColor || d.fill, d["stroke-width"] = w(this.options.edgeWidth, 1)); return d } var t = b.each, u = b.perspective, w = b.pick, x = b.Series, k = b.seriesTypes, n = b.inArray, f = b.svg, p = b.wrap; p(k.column.prototype, "translate", function (b) {
            b.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
                var d = this, a = d.chart, c = d.options, e = c.depth || 25, g = (c.stacking ? c.stack || 0 : d.index) *
                    (e + (c.groupZPadding || 1)), f = d.borderWidth % 2 ? .5 : 0; a.inverted && !d.yAxis.reversed && (f *= -1); !1 !== c.grouping && (g = 0); g += c.groupZPadding || 1; t(d.data, function (c) {
                        if (null !== c.y) {
                            var b = c.shapeArgs, h = c.tooltipPos, l; t([["x", "width"], ["y", "height"]], function (a) { l = b[a[0]] - f; 0 > l && (b[a[1]] += b[a[0]] + f, b[a[0]] = -f, l = 0); l + b[a[1]] > d[a[0] + "Axis"].len && 0 !== b[a[1]] && (b[a[1]] = d[a[0] + "Axis"].len - b[a[0]]); if (0 !== b[a[1]] && (b[a[0]] >= d[a[0] + "Axis"].len || b[a[0]] + b[a[1]] <= f)) for (var c in b) b[c] = 0 }); c.shapeType = "cuboid"; b.z = g; b.depth =
                                e; b.insidePlotArea = !0; h = u([{ x: h[0], y: h[1], z: g }], a, !0)[0]; c.tooltipPos = [h.x, h.y]
                        }
                    }); d.z = g
            }
        }); p(k.column.prototype, "animate", function (b) {
            if (this.chart.is3d()) {
                var d = arguments[1], a = this.yAxis, c = this, e = this.yAxis.reversed; f && (d ? t(c.data, function (c) { null !== c.y && (c.height = c.shapeArgs.height, c.shapey = c.shapeArgs.y, c.shapeArgs.height = 1, e || (c.shapeArgs.y = c.stackY ? c.plotY + a.translate(c.stackY) : c.plotY + (c.negative ? -c.height : c.height))) }) : (t(c.data, function (a) {
                null !== a.y && (a.shapeArgs.height = a.height, a.shapeArgs.y =
                    a.shapey, a.graphic && a.graphic.animate(a.shapeArgs, c.options.animation))
                }), this.drawDataLabels(), c.animate = null))
            } else b.apply(this, [].slice.call(arguments, 1))
        }); p(k.column.prototype, "plotGroup", function (b, d, a, c, e, g) { this.chart.is3d() && g && !this[d] && (this[d] = g, g.attr(this.getPlotBox()), this[d].survive = !0); return b.apply(this, Array.prototype.slice.call(arguments, 1)) }); p(k.column.prototype, "setVisible", function (b, d) {
            var a = this, c; a.chart.is3d() && t(a.data, function (b) {
                c = (b.visible = b.options.visible = d = void 0 ===
                    d ? !b.visible : d) ? "visible" : "hidden"; a.options.data[n(b, a.data)] = b.options; b.graphic && b.graphic.attr({ visibility: c })
            }); b.apply(this, Array.prototype.slice.call(arguments, 1))
        }); p(k.column.prototype, "init", function (b) {
            b.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
                var d = this.options, a = d.grouping, c = d.stacking, e = w(this.yAxis.options.reversedStacks, !0), g = 0; if (void 0 === a || a) {
                    a = this.chart.retrieveStacks(c); g = d.stack || 0; for (c = 0; c < a[g].series.length && a[g].series[c] !== this; c++); g = 10 * (a.totalStacks -
                        a[g].position) + (e ? c : -c); this.xAxis.reversed || (g = 10 * a.totalStacks - g)
                } d.zIndex = g
            }
        }); p(k.column.prototype, "pointAttribs", v); k.columnrange && (p(k.columnrange.prototype, "pointAttribs", v), k.columnrange.prototype.plotGroup = k.column.prototype.plotGroup, k.columnrange.prototype.setVisible = k.column.prototype.setVisible); p(x.prototype, "alignDataLabel", function (b) {
            if (this.chart.is3d() && ("column" === this.type || "columnrange" === this.type)) {
                var d = arguments[4], a = { x: d.x, y: d.y, z: this.z }, a = u([a], this.chart, !0)[0]; d.x = a.x;
                d.y = a.y
            } b.apply(this, [].slice.call(arguments, 1))
        }); p(b.StackItem.prototype, "getStackBox", function (f, d) { var a = f.apply(this, [].slice.call(arguments, 1)); if (d.is3d()) { var c = { x: a.x, y: a.y, z: 0 }, c = b.perspective([c], d, !0)[0]; a.x = c.x; a.y = c.y } return a })
    })(y); (function (b) {
        var v = b.deg2rad, t = b.each, u = b.pick, w = b.seriesTypes, x = b.svg; b = b.wrap; b(w.pie.prototype, "translate", function (b) {
            b.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
                var k = this, f = k.options, p = f.depth || 0, l = k.chart.options.chart.options3d,
                d = l.alpha, a = l.beta, c = f.stacking ? (f.stack || 0) * p : k._i * p, c = c + p / 2; !1 !== f.grouping && (c = 0); t(k.data, function (b) { var e = b.shapeArgs; b.shapeType = "arc3d"; e.z = c; e.depth = .75 * p; e.alpha = d; e.beta = a; e.center = k.center; e = (e.end + e.start) / 2; b.slicedTranslation = { translateX: Math.round(Math.cos(e) * f.slicedOffset * Math.cos(d * v)), translateY: Math.round(Math.sin(e) * f.slicedOffset * Math.cos(d * v)) } })
            }
        }); b(w.pie.prototype.pointClass.prototype, "haloPath", function (b) {
            var k = arguments; return this.series.chart.is3d() ? [] : b.call(this,
                k[1])
        }); b(w.pie.prototype, "pointAttribs", function (b, n, f) { b = b.call(this, n, f); f = this.options; this.chart.is3d() && (b.stroke = f.edgeColor || n.color || this.color, b["stroke-width"] = u(f.edgeWidth, 1)); return b }); b(w.pie.prototype, "drawPoints", function (b) { b.apply(this, [].slice.call(arguments, 1)); this.chart.is3d() && t(this.points, function (b) { var f = b.graphic; if (f) f[b.y && b.visible ? "show" : "hide"]() }) }); b(w.pie.prototype, "drawDataLabels", function (b) {
            if (this.chart.is3d()) {
                var k = this.chart.options.chart.options3d; t(this.data,
                    function (b) { var f = b.shapeArgs, l = f.r, d = (f.start + f.end) / 2, a = b.labelPos, c = -l * (1 - Math.cos((f.alpha || k.alpha) * v)) * Math.sin(d), e = l * (Math.cos((f.beta || k.beta) * v) - 1) * Math.cos(d); t([0, 2, 4], function (b) { a[b] += e; a[b + 1] += c }) })
            } b.apply(this, [].slice.call(arguments, 1))
        }); b(w.pie.prototype, "addPoint", function (b) { b.apply(this, [].slice.call(arguments, 1)); this.chart.is3d() && this.update(this.userOptions, !0) }); b(w.pie.prototype, "animate", function (b) {
            if (this.chart.is3d()) {
                var k = arguments[1], f = this.options.animation, p =
                    this.center, l = this.group, d = this.markerGroup; x && (!0 === f && (f = {}), k ? (l.oldtranslateX = l.translateX, l.oldtranslateY = l.translateY, k = { translateX: p[0], translateY: p[1], scaleX: .001, scaleY: .001 }, l.attr(k), d && (d.attrSetters = l.attrSetters, d.attr(k))) : (k = { translateX: l.oldtranslateX, translateY: l.oldtranslateY, scaleX: 1, scaleY: 1 }, l.animate(k, f), d && d.animate(k, f), this.animate = null))
            } else b.apply(this, [].slice.call(arguments, 1))
        })
    })(y); (function (b) {
        var v = b.perspective, t = b.pick, u = b.Point, w = b.seriesTypes, x = b.wrap; x(w.scatter.prototype,
            "translate", function (b) { b.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) { var k = this.chart, f = t(this.zAxis, k.options.zAxis[0]), p = [], l, d, a; for (a = 0; a < this.data.length; a++)l = this.data[a], d = f.isLog && f.val2lin ? f.val2lin(l.z) : l.z, l.plotZ = f.translate(d), l.isInside = l.isInside ? d >= f.min && d <= f.max : !1, p.push({ x: l.plotX, y: l.plotY, z: l.plotZ }); k = v(p, k, !0); for (a = 0; a < this.data.length; a++)l = this.data[a], f = k[a], l.plotXold = l.plotX, l.plotYold = l.plotY, l.plotZold = l.plotZ, l.plotX = f.x, l.plotY = f.y, l.plotZ = f.z } });
        x(w.scatter.prototype, "init", function (b, n, f) {
            n.is3d() && (this.axisTypes = ["xAxis", "yAxis", "zAxis"], this.pointArrayMap = ["x", "y", "z"], this.parallelArrays = ["x", "y", "z"], this.directTouch = !0); b = b.apply(this, [n, f]); this.chart.is3d() && (this.tooltipOptions.pointFormat = this.userOptions.tooltip ? this.userOptions.tooltip.pointFormat || "x: \x3cb\x3e{point.x}\x3c/b\x3e\x3cbr/\x3ey: \x3cb\x3e{point.y}\x3c/b\x3e\x3cbr/\x3ez: \x3cb\x3e{point.z}\x3c/b\x3e\x3cbr/\x3e" : "x: \x3cb\x3e{point.x}\x3c/b\x3e\x3cbr/\x3ey: \x3cb\x3e{point.y}\x3c/b\x3e\x3cbr/\x3ez: \x3cb\x3e{point.z}\x3c/b\x3e\x3cbr/\x3e");
            return b
        }); x(w.scatter.prototype, "pointAttribs", function (k, n) { var f = k.apply(this, [].slice.call(arguments, 1)); this.chart.is3d() && n && (f.zIndex = b.pointCameraDistance(n, this.chart)); return f }); x(u.prototype, "applyOptions", function (b) { var k = b.apply(this, [].slice.call(arguments, 1)); this.series.chart.is3d() && void 0 === k.z && (k.z = 0); return k })
    })(y); (function (b) {
        var v = b.Axis, t = b.SVGRenderer, u = b.VMLRenderer; u && (b.setOptions({ animate: !1 }), u.prototype.face3d = t.prototype.face3d, u.prototype.polyhedron = t.prototype.polyhedron,
            u.prototype.cuboid = t.prototype.cuboid, u.prototype.cuboidPath = t.prototype.cuboidPath, u.prototype.toLinePath = t.prototype.toLinePath, u.prototype.toLineSegments = t.prototype.toLineSegments, u.prototype.createElement3D = t.prototype.createElement3D, u.prototype.arc3d = function (b) { b = t.prototype.arc3d.call(this, b); b.css({ zIndex: b.zIndex }); return b }, b.VMLRenderer.prototype.arc3dPath = b.SVGRenderer.prototype.arc3dPath, b.wrap(v.prototype, "render", function (b) {
                b.apply(this, [].slice.call(arguments, 1)); this.sideFrame &&
                    (this.sideFrame.css({ zIndex: 0 }), this.sideFrame.front.attr({ fill: this.sideFrame.color })); this.bottomFrame && (this.bottomFrame.css({ zIndex: 1 }), this.bottomFrame.front.attr({ fill: this.bottomFrame.color })); this.backFrame && (this.backFrame.css({ zIndex: 0 }), this.backFrame.front.attr({ fill: this.backFrame.color }))
            }))
    })(y)
});