var Repeat = require('./lib/repeat');
var jQuery = require("jquery");
 if (parseFloat(jQuery.fn.VERSION) < 1.5) {
   throw new Error("Repeat.js requires jQuery 1.5 or later");
 }

Repeat.defer = function() {
  return jQuery.Deferred();
};

module.exports = Repeat;