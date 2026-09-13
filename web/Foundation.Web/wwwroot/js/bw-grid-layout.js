// Shared layouts for the operator grids (Trucks in Yard, Completed). A Manager
// or Admin arranges the columns — order, widths, which are shown, the default
// sort — and every user gets that arrangement. Anyone can still filter and
// sort for themselves; that is never saved. Reset puts the built-in layout
// back for everyone. With Require Login off, everyone may arrange it.
//
//   var layout = bwSharedGridLayout('completedTrucks');
//   ...dxDataGrid({ stateStoring: layout.stateStoring, onContentReady: layout.onContentReady, ... })
//   layout.attach(grid, function (resetButton) { /* add manager-only toolbar items */ });
(function (window, $) {
    'use strict';

    // The parts of a grid's state that belong to whoever set them. Pushing a
    // manager's filter onto every user would hide tickets from them.
    function shareable(state) {
        var s = $.extend(true, {}, state || {});
        ['filterValue', 'searchText', 'pageIndex', 'selectedRowKeys', 'filterPanel', 'focusedRowKey'].forEach(function (k) { delete s[k]; });
        (s.columns || []).forEach(function (c) {
            ['filterValue', 'filterValues', 'filterType', 'selectedFilterOperation'].forEach(function (k) { delete c[k]; });
        });
        return s;
    }

    window.bwSharedGridLayout = function (gridKey) {
        var url = '/api/grid-layouts/' + encodeURIComponent(gridKey);
        var info = $.getJSON(url);
        var lastSaved = null;
        var baselined = false;

        var api = {
            canEdit: false,

            stateStoring: {
                enabled: true,
                type: 'custom',
                savingTimeout: 1500,
                customLoad: function () {
                    // No saved layout, or the request failed: the grid's own
                    // column definitions are the layout.
                    return info.then(
                        function (r) { return r && r.state ? shareable(r.state) : null; },
                        function () { return null; });
                },
                customSave: function (state) {
                    if (!api.canEdit || !baselined) return;
                    var json = JSON.stringify(shareable(state));
                    if (json === lastSaved) return;
                    lastSaved = json;
                    $.ajax({ url: url, method: 'PUT', contentType: 'application/json', data: json })
                        .done(function () { bwNotify('Layout saved for all users', 'success', 1500); })
                        .fail(function () {
                            lastSaved = null;
                            bwNotify('The shared layout could not be saved.', 'error');
                        });
                }
            },

            // The layout as loaded is the starting point: only a change from
            // it is saved, so merely opening the page never writes anything.
            onContentReady: function (e) {
                if (baselined) return;
                baselined = true;
                lastSaved = JSON.stringify(shareable(e.component.state()));
            },

            // Manager/Admin: turn on the column tools and hand the caller a
            // Reset button to place in its toolbar.
            attach: function (grid, onEditable) {
                info.done(function (r) {
                    api.canEdit = !!(r && r.canEdit);
                    if (!api.canEdit) return;
                    grid.option('allowColumnReordering', true);
                    grid.option('allowColumnResizing', true);
                    grid.option('columnChooser.enabled', true);
                    if (onEditable) onEditable(api.resetButton());
                });
            },

            resetButton: function () {
                return {
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'revert',
                        text: 'Reset Layout',
                        hint: 'Put the columns back to the standard layout for every user',
                        onClick: function () { api.reset(); }
                    }
                };
            },

            reset: function () {
                return bwConfirm('Put the columns back to the standard layout? This changes the view for every user.',
                    'Reset Layout', { okText: 'Reset', danger: true })
                    .then(function (ok) {
                        if (!ok) return;
                        $.ajax({ url: url, method: 'DELETE' })
                            .done(function () { window.location.reload(); })
                            .fail(function () { bwNotify('The layout could not be reset.', 'error'); });
                    });
            }
        };
        return api;
    };
})(window, jQuery);
