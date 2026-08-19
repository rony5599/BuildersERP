// Progressive-enhancement AJAX pagination: any pagination link inside a
// [data-grid-container] element is fetched via XHR and swaps that container's
// contents, instead of doing a full page navigation. Falls back to a normal
// link (full page load) if JS is disabled or the fetch fails.
(function () {
    document.addEventListener('click', function (e) {
        var link = e.target.closest('.pagination-ajax .page-link');
        if (!link || !link.href) {
            return;
        }

        var container = link.closest('[data-grid-container]');
        if (!container) {
            return;
        }

        e.preventDefault();

        fetch(link.href, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error('Request failed: ' + response.status);
                }
                return response.text();
            })
            .then(function (html) {
                container.innerHTML = html;
                container.scrollIntoView({ behavior: 'smooth', block: 'start' });
                if (window.history && window.history.replaceState) {
                    window.history.replaceState(null, '', link.href);
                }
            })
            .catch(function () {
                window.location.href = link.href;
            });
    });
})();
