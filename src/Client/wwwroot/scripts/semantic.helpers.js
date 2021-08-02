(function () {
  window.semanticHelpers = {
    dropdown: function () {
      $('.ui.dropdown')
        .dropdown();
    },
    dropdownAllowAdditions: function () {
      $('.ui.dropdown.selection')
        .dropdown({
          allowAdditions: true
        });
    },
    toggleSidebar: function () {
      $(".ui.sidebar")
        .sidebar("toggle");
    },
    sidebar: function () {
      $('.left.sidebar')
        .sidebar({
          context: $('.bottom.segment')
        })
        .sidebar('attach events', '.toggle.button');
    },
    showDeleteConfirmation: function () {
      $('#deleteConfirmation')
        .modal('setting', 'closable', false)
        .modal('show');
    },
    rating: function () {
      $('.ui.rating')
        .rating();
    },
    tooltip: function () {
      $('.tooltip')
        .popup();
    },
  };
})();