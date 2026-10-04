// Mobile sidebar: hamburger toggle, backdrop click, Escape key, close on navigation.
(function () {
    const sidebar = document.getElementById('appSidebar');
    const toggle = document.getElementById('sidebarToggle');
    const backdrop = document.getElementById('sidebarBackdrop');
    if (!sidebar || !toggle) return;

    function setOpen(open) {
        sidebar.classList.toggle('is-open', open);
        document.body.classList.toggle('sidebar-open', open);
        toggle.setAttribute('aria-expanded', String(open));
    }

    toggle.addEventListener('click', function () {
        setOpen(!sidebar.classList.contains('is-open'));
    });

    if (backdrop) backdrop.addEventListener('click', function () { setOpen(false); });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') setOpen(false);
    });

    sidebar.querySelectorAll('a').forEach(function (a) {
        a.addEventListener('click', function () { setOpen(false); });
    });

    // Leaving the mobile breakpoint resets the state so the page never stays locked
    window.matchMedia('(min-width: 992px)').addEventListener('change', function (e) {
        if (e.matches) setOpen(false);
    });
})();

// Delete confirmation: any element with data-delete-url opens the shared modal
// (Views/Shared/_DeleteModal.cshtml) and points its form at that URL.
(function () {
    document.addEventListener('click', function (e) {
        const trigger = e.target.closest('[data-delete-url]');
        if (!trigger) return;

        const form = document.getElementById('deleteForm');
        const modalEl = document.getElementById('deleteModal');
        if (!form || !modalEl || typeof bootstrap === 'undefined') return;

        form.action = trigger.dataset.deleteUrl;
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    });
})();
