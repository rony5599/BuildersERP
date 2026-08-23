(function () {
    function initMcSelect(root) {
        var searchInput = root.querySelector('[data-mc-search]');
        var valueInput = root.querySelector('[data-mc-value]');
        var dropdown = root.querySelector('[data-mc-dropdown]');
        var body = root.querySelector('[data-mc-body]');
        var rows = Array.prototype.slice.call(body.querySelectorAll('[data-mc-option]'));
        var emptyMessage = dropdown.querySelector('.mc-select-empty');

        function open() {
            root.classList.add('open');
        }

        function close() {
            root.classList.remove('open');
        }

        function selectRow(row) {
            valueInput.value = row.getAttribute('data-value');
            searchInput.value = row.getAttribute('data-label');
            rows.forEach(function (r) { r.classList.remove('mc-active'); });
            row.classList.add('mc-active');
            close();
        }

        function filter() {
            var term = searchInput.value.trim().toLowerCase();
            var visibleCount = 0;
            rows.forEach(function (row) {
                var matches = !term || row.getAttribute('data-search').indexOf(term) !== -1;
                row.classList.toggle('mc-hidden', !matches);
                if (matches) visibleCount++;
            });
            if (emptyMessage) {
                emptyMessage.style.display = visibleCount === 0 ? '' : 'none';
            }
        }

        searchInput.addEventListener('focus', function () {
            open();
            filter();
        });

        searchInput.addEventListener('input', function () {
            valueInput.value = '';
            open();
            filter();
        });

        searchInput.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                close();
            } else if (e.key === 'Enter') {
                e.preventDefault();
                var visible = rows.filter(function (r) { return !r.classList.contains('mc-hidden'); });
                if (visible.length === 1) {
                    selectRow(visible[0]);
                }
            }
        });

        rows.forEach(function (row) {
            row.addEventListener('click', function () {
                selectRow(row);
            });
        });

        document.addEventListener('click', function (e) {
            if (!root.contains(e.target)) {
                close();
            }
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-mc-select]').forEach(initMcSelect);
    });
})();
