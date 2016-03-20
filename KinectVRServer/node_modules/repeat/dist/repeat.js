(function (global) {
  'use strict';
  var
      // --- UTILITY FUNCTIONS
      isFunction = function (obj) {
        return Object.prototype.toString.call(obj) === '[object Function]';
      },

      // Applies a callback to each key-value pair of
      // a object passing the key and value as parameters
      eachPair = function (obj, callback) {
        Object.keys(obj).forEach(function (key) {
          callback(key, obj[key]);
        });
      },

      // Supported numerals
      numerals = {
        one:1, two:2, three:3, four:4, five:5, six:6, seven:7, eight:8,
        nine:9, ten:10, twenty:20, fifty:50, hundred:100, thousand:1000
      },

      // Simple object extend based on underscore.js's _.extend()
      extend = function (obj) {
        Array.prototype.slice.call(arguments, 1).forEach(function (source) {
          for (var prop in source) {
            obj[prop] = source[prop];
          }
        });
        return obj;
      },

      // Supported units and their associated names/abbreviations
      unitsMap = (function () {
        var byName = {};
        [
          {ms:1, names:['millis', 'ms', 'milliseconds', 'millisecond']},
          {ms:1000, names:['s', 'sec', 'secs', 'seconds', 'second']},
          {ms:1000 * 60, names:['m', 'min', 'mins', 'minutes', 'minute']},
          {ms:1000 * 60 * 60, names:['h', 'hours', 'hour']}
        ].forEach(function (unit) {
              unit.names.forEach(function (name) {
                byName[name] = unit.ms;
              });
            });
        return byName;
      }()),

      // Creates a proxy around a function and turns it into a function that takes a number and a unit-name as parameters
      // The proxied function will be applied with the converted amount of milliseconds
      // Example:
      //  > var func = withUnitArgs(function(ms) { console.log("Given milliseconds:", ms); });
      //  > func(1, 'minute')
      //  => Given milliseconds: 60000
      //
      // It also ensures given unit is valid:
      //  > func(1, 'lightyear')
      // => Error: Unknown unit "lightyear" must be one of [millis, ms, milliseconds, millisecond, s, (...)]
      //
      // If no unit name is given, milliseconds is assumed
      withUnitArgs = function (func) {
        return function (val, unit) {
          if (unit && !unitsMap[unit]) {
            throw new Error('Unknown unit "' + unit + '" must be one of [' + Object.keys(unitsMap).join(", ") + ']');
          }
          return func.call(func, (unit && unitsMap[unit] * val) || val);
        };
      },
      // Takes a scope object and a function and make sure the function always returns the scope object
      // after its been invoked.
      //
      // Example:
      //  var scope = 1;
      //  var func = chained(scope, function(arg) { console.log(arg); });
      //  func('foo') == scope
      //  => true
      chained = function (instance, func) {
        return function () {
          func.apply(func, arguments);
          return instance;
        };
      };

  function Repeat(task) {
    var
        opts,
        timer,
        tick,
        results = [],
        tickCount = 0,
        started = false,
        completed = false,
        deferred = Repeat.defer(),
        self = extend({}, deferred.promise());

    opts = {
      task:task || null,
      every:-1,
      times:-1,
      for_:-1,
      until:null,
      if_:null,
      paused: false
    };

    self.task = chained(self, function (task) {
      opts.task = task;
    });

    self.every = chained(self, withUnitArgs(function (ms) {
      opts.every = ms;
    }));

    self['for'] = self.lasting = chained(self, withUnitArgs(function (ms) {
      opts.for_ = ms;
    }));

    self['if'] = self.provided = chained(self, function (func) {
      opts.if_ = func;
    });

    self.unless = chained(self, function (func) {
      opts.if_ = function () {
        return !func();
      };
    });

    self['while'] = self.during = chained(self, function (func) {
      opts.until = function () {
        return !func();
      };
    });

    self.times = chained(self, function (times) {
      opts.times = times;
    });

    self.until = chained(self, function (func) {
      opts.until = func;
    });

    tick = chained(self, function () {
      var result,
          nextTick = function() {
            if (~opts.every) {
              timer = setTimeout(tick, opts.every);
            }
          },
          mute = false,
          done = function (result) {
            results.push(result);
            if (!mute) deferred.notify(result);
            nextTick();
          },
          skip = opts.paused === true || (isFunction(opts.if_) && !opts.if_());

      if (completed) {
        return;
      }

      if (!started) {
        started = +(new Date());
      }

      if (isFunction(opts.until) && opts.until()) {
        self.cancel();
      } else if (~opts.for_ && +(new Date()) - started > opts.for_) {
        self.cancel();
      } else if (~opts.times && tickCount === opts.times) {
        self.cancel();
      }
      else if (opts.paused === true) {
        nextTick();
      }
      else {
        var scope = {mute: function() {mute = true;}};
        if (opts.task.length > 0) { // first argument of function should be the done() callback
          if (!skip) {
            try {
              opts.task.call(scope, done);
            } catch (e) {
              deferred.reject(e);
              throw e;
            }
          }
          tickCount++;
        }
        else {
          if (!skip) {
            try {
              result = opts.task.call(scope);
            } catch (e) {
              deferred.reject(e);
              throw e;
            }
          }
          tickCount++;
          done(result);
        }
      }
    });

    self.pause = chained(self, function() {
      opts.paused = true;
    });

    self.resume = chained(self, function() {
      opts.paused = false;
    });

    self.start = function () {

      if (started) {
        throw new Error("Already started");
      } else if (!opts.task) {
        throw new Error("Don't know any task to run");
      } else if (!isFunction(opts.task)) {
        throw new Error("Uh oh, the given task is not a function");
      }

      if (~opts.every) { // we're running in asynchronous mode
        setTimeout(function () { // this timeout is to ensure that first task invocation is carried out asynchronously
          tick();
        }, 0);
      }
      else {  // we're running in synchronous mode
        //        if (!~opts.times && !isFunction(opts.until)) {
        //          // oops, this would cause an infinite loop
        //          throw new Error("Cannot run synchronously without knowing how many times. Please specify stop condition, number of times to run, or run asynchronous.");
        //        }
        do {
          tick();
        } while (!completed);
      }
      return deferred.promise();
    };

    self.start['in'] = self['in'] = self.wait = withUnitArgs(function (ms) {
      timer = setTimeout(function () {
        self.start();
      }, ms);
      return deferred.promise();
    });

    self.next = chained(self, function () {
      tick();
    });

    self.stop = chained(self, function () {
      if (!started) {
        self.cancel();
      }
      opts.times = tickCount + 1; // do not cancel if between two invocations
    });

    self.cancel = chained(self, function () {
      if (timer) {
        clearTimeout(timer);
      }
      deferred.resolve(results);
      completed = true;
    });

    self.async = chained(self, function () {
      return self.every(0, 'ms');
    });

    // Expose predefined numerals as api functions
    eachPair(numerals, function (key, value) {
      self[key] = {times:function () {
        self.times(value);
        return self;
      }};
    });

    // Add some grammatical convenience
    self.once = self.one.time = self.one.times;
    self.twice = self.two.times;
    self.now = self.start.now = self.start;

    return self;
  }

  if (typeof global != 'undefined') {
    Repeat.defer = global.jQuery ?
        function() { return new global.jQuery.Deferred(); } :
        global.Deferred ?
            function() { return global.Deferred(); } : null;
  }

  // Finally, export it as CommonJS module *or* to to the global object as Repeat
  if (typeof exports !== 'undefined') {
    module.exports = Repeat; // CommonJS
  }
  else {
    // Export Repeat to global object
    global.Repeat = Repeat;
  }

}(this));
