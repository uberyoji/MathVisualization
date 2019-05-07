mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },
  
  AddNumbers: function (x, y) {
    return x + y;
  },
  
  GetGlobal: function () {
    return global;
  }

});