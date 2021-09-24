/**
 * Display the pager for table.
 */
app.component('pager', {
  bindings: {
    ngFilter: '&'
  },
  transclude: true,
  templateUrl: '/content/components/_AngularTablePager.html',
  controller: function ($scope, $element, $attrs, $timeout, Utils) {
    var $ctrl = this;
    this.pageSizes = $attrs.ngPageSize ? JSON.parse($attrs.ngPageSize) : [5, 10, 20];
    this.pageHeader = $attrs.ngPageHeader ? JSON.parse($attrs.ngPageHeader) : true;
    this.pageSearch = $attrs.ngPageSearch ? JSON.parse($attrs.ngPageSearch) : true;

    this.updatePager = function () {
      $ctrl.page = 1;
      $ctrl.pages = [];
      $ctrl.gotoPage($ctrl.page);
    };

    this.gotoPage = function (page) {
      if ($attrs.ngLocal) {
        $ctrl.pageTotal = $ctrl.ngItems.length;
        updatePager(page);
      } else {
        $ctrl.updateItems(page);
      }
    };

    function updatePager(page) {
      var pager = Utils.setupPager(page, $ctrl.pageSize, $ctrl.pageTotal, $ctrl.ngItems);
      Utils.copyWhere(pager, $ctrl);
      $ctrl.pageItemMin = ($ctrl.page - 1) * $ctrl.pageSize + 1;
      $ctrl.pageItemMax = $ctrl.page * $ctrl.pageSize;
      $ctrl.pages = pager.pages;
    }

    this.showPage = function (index) {
      var number = index + 1;
      return number >= $ctrl.pageItemMin && number <= $ctrl.pageItemMax;
    };

    this.updateItems = function (page) {
      if ($attrs.ngLocal) {
        $ctrl.ngItems = $ctrl.ngFilter()($ctrl.ngKeyword);
        $ctrl.updatePager();
      } else {
        $ctrl.page = page || 1;
        $ctrl.ngFilter()($ctrl.ngKeyword, $ctrl.page, $ctrl.pageSize).then(function (response) {
          $timeout(function () {
            $ctrl.ngItems = response.Data;
            $ctrl.pageTotal = response.RecordsTotal;
            updatePager($ctrl.page);
          });
        });
      }
    };

    this.$onInit = function () {
      $ctrl.pageSize = $ctrl.pageSize || $ctrl.pageSizes[0];
      $ctrl.updateItems();
    };

    $scope.$on('refreshPager', function () {
      $ctrl.updateItems($ctrl.page);
    });
  }
});
