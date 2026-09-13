// Site-wide alert / confirm / notice dialogs, built on DevExtreme so every
// popup in the office screens looks and behaves the same. Replaces the
// browser's native alert() and confirm(), which render differently in every
// browser, block the page, and can be suppressed by the user.
//
//   bwAlert('Saved.')                                  -> Promise<void>
//   bwConfirm('Delete this scale?').then(function (ok) { if (ok) ... })
//   bwConfirm('Delete ticket 1234?', 'Delete Ticket',
//             { okText: 'Delete', danger: true })       -> Promise<boolean>
//   bwNotify('Printed', 'success')                     -> toast, no button
//
// Messages are plain text (escaped, newlines become line breaks). Pass
// { html: true } to render trusted markup instead.
(function (window) {
    'use strict';

    function esc(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    function body(message, opts) {
        var html = opts && opts.html ? String(message) : esc(message).replace(/\r?\n/g, '<br>');
        return '<div class="bw-dialog-message">' + html + '</div>';
    }

    function dx() {
        return window.DevExpress && window.DevExpress.ui && window.DevExpress.ui.dialog;
    }

    // Wraps DevExtreme's jQuery Deferred so callers get a real Promise and
    // can use either .then() or await.
    function show(title, message, buttons, opts) {
        if (!dx()) {
            // DevExtreme not on this page — fall back rather than lose the
            // message. Every page on the shared layout has it.
            if (buttons.length > 1) return Promise.resolve(window.confirm(message));
            window.alert(message);
            return Promise.resolve(true);
        }
        var d = window.DevExpress.ui.dialog.custom({
            title: title,
            messageHtml: body(message, opts),
            buttons: buttons,
            dragEnabled: false,
            showTitle: true
        });
        // Closing with Esc rejects DevExtreme's deferred instead of resolving
        // it; treat that as the dismissive answer so callers always hear back.
        return new Promise(function (resolve) {
            d.show()
                .done(function (result) { resolve(result); })
                .fail(function () { resolve(undefined); });
        });
    }

    window.bwAlert = function (message, title, opts) {
        opts = opts || {};
        return show(title || 'Notice', message, [
            { text: opts.okText || 'OK', type: opts.type || 'default', stylingMode: 'contained', onClick: function () { return true; } }
        ], opts).then(function () { });
    };

    window.bwConfirm = function (message, title, opts) {
        opts = opts || {};
        return show(title || 'Confirm', message, [
            { text: opts.okText || 'OK', type: opts.danger ? 'danger' : 'default', stylingMode: 'contained', onClick: function () { return true; } },
            { text: opts.cancelText || 'Cancel', type: 'normal', stylingMode: 'outlined', onClick: function () { return false; } }
        ], opts).then(function (r) { return r === true; });
    };

    // A toast in the top-right corner, for results that need no answer.
    // type: 'success' | 'info' | 'warning' | 'error' ('danger' is accepted).
    window.bwNotify = function (message, type, displayTime) {
        if (type === 'danger') type = 'error';
        if (!window.DevExpress || !window.DevExpress.ui || !window.DevExpress.ui.notify) {
            window.alert(message);
            return;
        }
        window.DevExpress.ui.notify({
            message: message,
            type: type || 'info',
            displayTime: displayTime || (type === 'error' ? 6000 : 3500),
            width: 'auto',
            maxWidth: 480
        }, { position: 'top right', direction: 'down-push' });
    };
})(window);
