// Minimal touch/mouse signature pad shared by the Weigh Out capture overlay
// (Operator mode) and the /SignaturePad remote tablet page (RemotePad mode).
// No external dependencies so it works on offline scale-house networks.
//
//   var pad = new BwSignaturePad(canvasElement);
//   pad.clear(); pad.isEmpty(); pad.toDataURL(); pad.resize();
//
// Strokes are kept in CSS-pixel coordinates and redrawn on resize, so a
// rotation or window resize mid-signature doesn't wipe the drawing.
(function () {
    'use strict';

    function BwSignaturePad(canvas) {
        var ctx = canvas.getContext('2d');
        var strokes = [];      // finished strokes: arrays of {x, y}
        var current = null;    // in-progress stroke
        var ratio = 1;

        function applyInkStyle() {
            ctx.strokeStyle = '#000';
            ctx.fillStyle = '#000';
            ctx.lineWidth = 2.5;
            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';
        }

        function drawStrokeOn(c, pts) {
            if (!pts.length) return;
            if (pts.length < 3) {
                c.beginPath();
                c.arc(pts[0].x, pts[0].y, c.lineWidth / 2, 0, Math.PI * 2);
                c.fill();
                return;
            }
            // Quadratic curves through midpoints smooth out the polyline
            c.beginPath();
            c.moveTo(pts[0].x, pts[0].y);
            for (var i = 1; i < pts.length - 1; i++) {
                var midX = (pts[i].x + pts[i + 1].x) / 2;
                var midY = (pts[i].y + pts[i + 1].y) / 2;
                c.quadraticCurveTo(pts[i].x, pts[i].y, midX, midY);
            }
            c.lineTo(pts[pts.length - 1].x, pts[pts.length - 1].y);
            c.stroke();
        }

        function drawStroke(pts) { drawStrokeOn(ctx, pts); }

        function redraw() {
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.scale(ratio, ratio);
            applyInkStyle();
            strokes.forEach(drawStroke);
            if (current) drawStroke(current);
        }

        function resize() {
            ratio = Math.max(window.devicePixelRatio || 1, 1);
            var rect = canvas.getBoundingClientRect();
            canvas.width = Math.max(1, Math.round(rect.width * ratio));
            canvas.height = Math.max(1, Math.round(rect.height * ratio));
            redraw();
        }

        function pointFrom(e) {
            var rect = canvas.getBoundingClientRect();
            return { x: e.clientX - rect.left, y: e.clientY - rect.top };
        }

        canvas.style.touchAction = 'none'; // stop the browser panning while signing

        canvas.addEventListener('pointerdown', function (e) {
            e.preventDefault();
            canvas.setPointerCapture(e.pointerId);
            current = [pointFrom(e)];
            redraw();
        });

        canvas.addEventListener('pointermove', function (e) {
            if (!current) return;
            e.preventDefault();
            // Use coalesced events when available for smoother fast strokes.
            // Some implementations return an empty list — fall back to the
            // event itself or the move is silently lost.
            var events = e.getCoalescedEvents ? e.getCoalescedEvents() : [];
            if (!events.length) events = [e];
            for (var i = 0; i < events.length; i++) current.push(pointFrom(events[i]));
            redraw();
        });

        function endStroke(e) {
            if (!current) return;
            e.preventDefault();
            strokes.push(current);
            current = null;
        }
        canvas.addEventListener('pointerup', endStroke);
        canvas.addEventListener('pointercancel', endStroke);

        window.addEventListener('resize', resize);
        resize();

        this.clear = function () {
            strokes = [];
            current = null;
            redraw();
        };
        this.isEmpty = function () {
            return strokes.length === 0 && !current;
        };
        // Export cropped to the ink and redrawn with a bold pen. The raw
        // canvas is mostly empty space, so a zoom-to-fit picture box on the
        // printed ticket shrank the actual signature to a thin dithered
        // scrawl on 1-bit thermal printers. Cropping makes the ink fill the
        // box; the fixed bold pen width survives thermal rasterization.
        this.toDataURL = function () {
            var all = strokes.slice();
            if (current) all.push(current);
            if (!all.length) return canvas.toDataURL('image/png');

            var minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;
            all.forEach(function (s) {
                s.forEach(function (p) {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                });
            });

            var w = Math.max(maxX - minX, 1);
            var h = Math.max(maxY - minY, 1);
            var TARGET_H = 300;   // export ink height in pixels
            var MAX_W = 1400;
            var scale = Math.min(TARGET_H / h, MAX_W / w);
            var PEN = 9;          // bold, in export pixels — constant regardless of scale
            var pad = PEN;        // keep round caps inside the canvas

            var out = document.createElement('canvas');
            out.width = Math.ceil(w * scale) + pad * 2;
            out.height = Math.ceil(h * scale) + pad * 2;
            var octx = out.getContext('2d');
            octx.strokeStyle = '#000';
            octx.fillStyle = '#000';
            octx.lineWidth = PEN;
            octx.lineCap = 'round';
            octx.lineJoin = 'round';

            all.forEach(function (pts) {
                drawStrokeOn(octx, pts.map(function (p) {
                    return { x: (p.x - minX) * scale + pad, y: (p.y - minY) * scale + pad };
                }));
            });
            return out.toDataURL('image/png');
        };
        this.resize = resize;
    }

    window.BwSignaturePad = BwSignaturePad;
})();
