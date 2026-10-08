(function () {
    function init(select) {
        if (!select || select.dataset.workGroupPickerReady === 'true') return;
        select.dataset.workGroupPickerReady = 'true';
        select.classList.add('work-group-picker-native');

        const root = document.createElement('div');
        root.className = 'work-group-picker';
        root.innerHTML = '<button type="button" class="work-group-picker__trigger" aria-haspopup="listbox" aria-expanded="false"></button>' +
            '<div class="work-group-picker__panel"><div class="work-group-picker__search-wrap"><span class="work-group-picker__search-icon">⌕</span><input class="work-group-picker__search" type="search" placeholder="Search by code or work group…" autocomplete="off"></div><div class="work-group-picker__list" role="listbox"></div><div class="work-group-picker__empty">No matching work groups</div></div>';
        select.insertAdjacentElement('afterend', root);

        const trigger = root.querySelector('.work-group-picker__trigger');
        const panel = root.querySelector('.work-group-picker__panel');
        const search = root.querySelector('.work-group-picker__search');
        const list = root.querySelector('.work-group-picker__list');
        const empty = root.querySelector('.work-group-picker__empty');
        document.body.appendChild(panel);
        const options = Array.from(select.options).filter(option => option.value).map(option => ({
            option,
            value: option.value,
            code: option.dataset.code || '',
            name: option.dataset.name || option.textContent.trim(),
            depth: Number(option.dataset.depth || 0)
        }));

        options.forEach(item => {
            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'work-group-picker__option' + (item.depth === 0 ? ' is-parent' : '');
            button.dataset.value = item.value;
            button.dataset.search = `${item.code} ${item.name}`.toLowerCase();
            button.style.paddingLeft = `${.6 + item.depth * 1.05}rem`;
            button.innerHTML = `<span class="work-group-picker__option-code">${escapeHtml(item.code)}</span><span class="work-group-picker__option-name">${escapeHtml(item.name)}</span><span class="work-group-picker__option-check">✓</span>`;
            button.addEventListener('click', () => {
                select.value = item.value;
                select.dispatchEvent(new Event('change', { bubbles: true }));
                renderValue();
                close();
            });
            list.appendChild(button);
        });

        function escapeHtml(value) {
            const span = document.createElement('span');
            span.textContent = value;
            return span.innerHTML;
        }
        function renderValue() {
            const item = options.find(x => x.value === select.value);
            trigger.innerHTML = item
                ? `<span class="work-group-picker__code">${escapeHtml(item.code)}</span><span class="work-group-picker__label">${escapeHtml(item.name)}</span><span class="work-group-picker__chevron">▾</span>`
                : '<span class="work-group-picker__label work-group-picker__placeholder">Select work group</span><span class="work-group-picker__chevron">▾</span>';
            list.querySelectorAll('.work-group-picker__option').forEach(x => x.classList.toggle('is-active', x.dataset.value === select.value));
        }
        function open() {
            document.querySelectorAll('.work-group-picker.is-open').forEach(x => { if (x !== root) x.classList.remove('is-open'); });
            document.querySelectorAll('.work-group-picker__panel.is-open').forEach(x => { if (x !== panel) x.classList.remove('is-open'); });
            positionPanel();
            root.classList.add('is-open');
            panel.classList.add('is-open');
            trigger.setAttribute('aria-expanded', 'true');
            search.value = '';
            filter();
            setTimeout(() => search.focus(), 0);
        }
        function positionPanel() {
            const bounds = trigger.getBoundingClientRect();
            const panelWidth = Math.min(Math.max(bounds.width, 340), 440);
            const left = Math.min(bounds.left, window.innerWidth - panelWidth - 16);
            panel.style.left = `${Math.max(16, left)}px`;
            panel.style.width = `${panelWidth}px`;
            const roomBelow = window.innerHeight - bounds.bottom;
            const openUpward = roomBelow < 360 && bounds.top > roomBelow;
            panel.style.top = openUpward ? 'auto' : `${bounds.bottom + 6}px`;
            panel.style.bottom = openUpward ? `${window.innerHeight - bounds.top + 6}px` : 'auto';
        }
        function close() {
            root.classList.remove('is-open');
            panel.classList.remove('is-open');
            trigger.setAttribute('aria-expanded', 'false');
        }
        function filter() {
            const term = search.value.trim().toLowerCase();
            let visible = 0;
            list.querySelectorAll('.work-group-picker__option').forEach(option => {
                const matches = !term || option.dataset.search.includes(term);
                option.hidden = !matches;
                if (matches) visible++;
            });
            empty.style.display = visible ? 'none' : 'block';
        }

        trigger.addEventListener('click', () => root.classList.contains('is-open') ? close() : open());
        search.addEventListener('input', filter);
        search.addEventListener('keydown', event => { if (event.key === 'Escape') { close(); trigger.focus(); } });
        document.addEventListener('click', event => { if (!root.contains(event.target) && !panel.contains(event.target)) close(); });
        window.addEventListener('resize', () => { if (root.classList.contains('is-open')) positionPanel(); });
        window.addEventListener('scroll', event => {
            if (!root.classList.contains('is-open') || panel.contains(event.target)) return;
            positionPanel();
        }, true);
        renderValue();
    }

    window.WorkGroupPicker = { init };
    document.addEventListener('DOMContentLoaded', () => document.querySelectorAll('select[data-work-group-picker]').forEach(init));
})();
