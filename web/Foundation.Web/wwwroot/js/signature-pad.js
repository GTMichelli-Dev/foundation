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

        function drawStroke(pts) {
            if (!pts.length) return;
            if (pts.length < 3) {
                ctx.beginPath();
                ctx.arc(pts[0].x, pts[0].y, ctx.lineWidth / 2, 0, Math.PI * 2);
                ctx.fill();
                return;
            }
            // Quadratic curves through midpoints smooth out the polyline
            ctx.beginPath();
            ctx.moveTo(pts[0].x, pts[0].y);
            for (var i = 1; i < pts.length - 1; i++) {
                var midX = (pts[i].x + pts[i + 1].x) / 2;
                var midY = (pts[i].y + pts[i + 1].y) / 2;
                ctx.quadraticCurveTo(pts[i].x, pts[i].y, midX, midY);
            }
            ctx.lineTo(pts[pts.length - 1].x, pts[pts.length - 1].y);
            ctx.stroke();
        }

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
            // Use coalesced events when available for smoother fast strokes
            var events = e.getCoalescedEvents ? e.getCoalescedEvents() : [e];
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
        this.toDataURL = function () {
            return canvas.toDataURL('image/png');
        };
        this.resize = resize;
    }

    window.BwSignaturePad = BwSignaturePad;
})();
