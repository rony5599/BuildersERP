/**
 * Multi-Column Dropdown Component
 * Supports filtering across multiple columns
 */

class MultiColumnDropdown {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        if (!this.container) return;

        this.dropdownId = containerId.replace('-container', '');
        this.input = document.getElementById(this.dropdownId);
        this.filter = document.getElementById(this.dropdownId + '-filter');
        this.menu = document.getElementById(this.dropdownId + '-menu');
        this.items = this.menu?.querySelectorAll('.multicolumn-dropdown-item') || [];

        this.isOpen = false;
        this.selectedItem = null;
        this.allItems = Array.from(this.items);

        this.init();
    }

    init() {
        if (!this.filter || !this.menu) return;

        // Filter input
        this.filter.addEventListener('input', (e) => this.handleFilter(e));
        this.filter.addEventListener('focus', () => this.open());
        this.filter.addEventListener('keydown', (e) => this.handleKeydown(e));

        // Item selection
        this.items.forEach(item => {
            item.addEventListener('click', () => this.selectItem(item));
        });

        // Close on outside click
        document.addEventListener('click', (e) => {
            if (!this.container.contains(e.target)) {
                this.close();
            }
        });

        // Set initial display value
        this.updateDisplay();
    }

    handleFilter(e) {
        const query = e.target.value.toLowerCase();
        let visibleCount = 0;

        this.items.forEach(item => {
            const code = item.querySelector('.multicolumn-dropdown-column:nth-child(1)')?.textContent?.toLowerCase() || '';
            const name = item.querySelector('.multicolumn-dropdown-column:nth-child(2)')?.textContent?.toLowerCase() || '';
            const category = item.querySelector('.multicolumn-dropdown-column:nth-child(3)')?.textContent?.toLowerCase() || '';

            const matches = code.includes(query) || name.includes(query) || category.includes(query);

            if (matches && query.length >= 0) {
                item.classList.remove('hidden');
                visibleCount++;
            } else {
                item.classList.add('hidden');
            }
        });

        // Show empty state if no results
        this.updateEmptyState(visibleCount === 0);
        this.open();
    }

    updateEmptyState(isEmpty) {
        let emptyState = this.menu?.querySelector('.multicolumn-dropdown-empty');
        
        if (isEmpty && !emptyState) {
            emptyState = document.createElement('div');
            emptyState.className = 'multicolumn-dropdown-empty';
            emptyState.textContent = 'No results found';
            this.menu.appendChild(emptyState);
        } else if (!isEmpty && emptyState) {
            emptyState.remove();
        }
    }

    handleKeydown(e) {
        if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
            e.preventDefault();
            this.navigateItems(e.key === 'ArrowDown' ? 1 : -1);
        } else if (e.key === 'Enter') {
            e.preventDefault();
            const focusedItem = this.menu?.querySelector('.multicolumn-dropdown-item:focus');
            if (focusedItem) {
                this.selectItem(focusedItem);
            }
        } else if (e.key === 'Escape') {
            this.close();
        }
    }

    navigateItems(direction) {
        const visibleItems = Array.from(this.items).filter(item => !item.classList.contains('hidden'));
        if (visibleItems.length === 0) return;

        let currentIndex = visibleItems.indexOf(document.activeElement);
        let nextIndex = direction === 1 ? currentIndex + 1 : currentIndex - 1;

        if (nextIndex < 0) nextIndex = visibleItems.length - 1;
        if (nextIndex >= visibleItems.length) nextIndex = 0;

        visibleItems[nextIndex].focus();
    }

    selectItem(item) {
        const value = item.dataset.value;
        const code = item.querySelector('.multicolumn-dropdown-column:nth-child(1)')?.textContent?.trim() || '';
        const name = item.querySelector('.multicolumn-dropdown-column:nth-child(2)')?.textContent?.trim() || '';

        // Update hidden input
        this.input.value = value;

        // Update filter display - show Code - Name
        this.filter.value = `${code} - ${name}`.trim();

        // Update selected state
        this.items.forEach(i => i.classList.remove('selected'));
        item.classList.add('selected');

        this.selectedItem = item;
        this.close();

        // Trigger change event
        this.input.dispatchEvent(new Event('change', { bubbles: true }));
    }

    updateDisplay() {
        const value = this.input.value;
        if (value) {
            const selected = Array.from(this.items).find(item => item.dataset.value === value);
            if (selected) {
                selected.classList.add('selected');
                const code = selected.querySelector('.multicolumn-dropdown-column:nth-child(1)')?.textContent?.trim() || '';
                const name = selected.querySelector('.multicolumn-dropdown-column:nth-child(2)')?.textContent?.trim() || '';
                this.filter.value = `${code} - ${name}`.trim();
                this.selectedItem = selected;
            }
        }
    }

    open() {
        if (!this.isOpen) {
            this.menu.style.display = 'block';
            this.isOpen = true;
        }
    }

    close() {
        if (this.isOpen) {
            this.menu.style.display = 'none';
            this.isOpen = false;
            if (this.selectedItem) {
                const code = this.selectedItem.querySelector('.multicolumn-dropdown-column:nth-child(1)')?.textContent?.trim() || '';
                const name = this.selectedItem.querySelector('.multicolumn-dropdown-column:nth-child(2)')?.textContent?.trim() || '';
                this.filter.value = `${code} - ${name}`.trim();
            }
        }
    }

    toggle() {
        if (this.isOpen) {
            this.close();
        } else {
            this.open();
        }
    }
}

// Auto-initialize all multicolumn dropdowns
document.addEventListener('DOMContentLoaded', function () {
    const containers = document.querySelectorAll('[id$="-container"]');
    containers.forEach(container => {
        if (container.id.includes('multicolumn-dropdown')) {
            new MultiColumnDropdown(container.id);
        }
    });
});

// Export for manual initialization
window.MultiColumnDropdown = MultiColumnDropdown;
