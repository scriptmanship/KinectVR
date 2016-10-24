var Repeat = module.exports = require('./lib/repeat');

var Deferred = require('dfrrd');
Repeat.defer = function() {
  return new Deferred();
};