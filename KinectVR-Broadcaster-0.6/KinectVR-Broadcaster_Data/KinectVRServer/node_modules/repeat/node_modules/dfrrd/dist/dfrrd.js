(function (global) {
  'use strict';
  var
    // -- Utils
    slice = Array.prototype.slice,
    isFunction = function (obj) {
      return Object.prototype.toString.call(obj) === '[object Function]';
    },
    invokeEach = function (funcs, args) {
      if (typeof funcs === 'undefined' || funcs.length === 0) { return; }
      for (var i = 0, len = funcs.length; i < len; i++) {
        var func = funcs[i];
        func.apply(func, args);
      }
    },

    // -- Friends of Deferred
    events = ['resolve', 'reject', 'notify', 'done'], // supported events
    addListener = function (callbacks/*, then, fail, notify, done*/) {
      var i = 0, event, listener, len = events.length;
      for (; i < arguments.length && i < len; i++) {
        listener = arguments[i+1];

        if (!isFunction(listener)) { continue; }

        event = events[i];
        if (!callbacks.hasOwnProperty(event)) {
          callbacks[event] = [];
        }
        callbacks[event].push(listener);
      }
    },
    finalize = function (promise) {
      promise.resolve = promise.reject = function () { throw new Error('Promise already completed'); };
    },
    notify = function (callbacks, type, values) {
      invokeEach((callbacks[type] || []).concat(callbacks.done || []), values);
    },
    synchronize = function (promise, type, values) {
      promise.then = function (success, error, progress, always) {  // Switch to synchronized
        if (type === 'resolve') { success.apply(success, values); }
        else if (isFunction(error)) { error.apply(error, values); }
        if (isFunction(always)) { always.apply(always, values); }
      };
    },
    makePromise = (function () {
      function Promise() {}
      return function (deferred) {
        var p = new Promise();
        p.then = function () {
          return deferred.then.apply(deferred, arguments);
        };
        p.fail = function () {
          return deferred.fail.apply(deferred, arguments);
        };
        p.progress = function () {
          return deferred.progress.apply(deferred, arguments);
        };
        p.always = function () {
          return deferred.always.apply(deferred, arguments); 
        };
        return p;
      };
    })();

  var Deferred = (function () {
    function Deferred() {

      if (!(this instanceof Deferred)) { return new Deferred(); }

      var
        _this = this,
        callbacks = {},
        state = 'pending',
        notifications = [],
        fulfill = function (type, values) {
          state = type === 'resolve' ? 'resolved' : 'rejected';
          finalize(_this);
          synchronize(_this, type, values);
          notify(callbacks, type, values);
          callbacks = null;
        };

      // -- Deferred API
      this.state = function() {
        return state;
      };
      this.resolve = function (/*value1, ..., valueN */) {
        fulfill('resolve', arguments);
      };
      this.reject = function (/*value1, ..., valueN */) {
        fulfill('reject', arguments);
      };
      this.notify = function (/*value1, ..., valueN */) {
        notifications.push(arguments);
        invokeEach(callbacks.notify, arguments);
      };

      // -- Promise API
      this.then = function (success, error, progress, always) {
        addListener(callbacks, success, error, progress, always);
        if (progress && notifications.length > 0) {
          notifications.forEach(function(args) {
            progress.apply(null, args)
          });
        }
        return this;
      };
    }

    // -- More promise API (only convenience functions)
    Deferred.prototype = {
      fail: function (callback) {
        return this.then(null, callback);
      },
      progress: function (callback) {
        return this.then(null, null, callback);
      },
      always: function (callback) { 
        return this.then(null, null, null, callback);
      },
      promise: function () {
        return makePromise(this);
      }
    };

    // --- Adds chainability
    (function() {
      var saveResult = function (memory, type, index, values) {
        memory[type][index] = values;
        memory[type === 'resolved' ? 'rejected' : 'resolved'][index] = undefined;
        memory.count[type]++;
      },
      check = function (memory, deferred) {
        var completedCount = memory.count.resolved + memory.count.rejected;
        if (completedCount === memory.count.total) {
          if (memory.count.resolved === memory.count.total) {
            deferred.resolve.apply(deferred, memory.resolved);
          }
          else {
            deferred.reject.apply(deferred, memory.rejected);
          }
        }
      },
      join = function (memory, deferred, promise) {
        var index = memory.count.total++;
        promise.then(
            function(/* value1, ..., valueN */) {
              saveResult(memory, 'resolved', index, slice.apply(arguments));
              check(memory, deferred);
            },
            function(/* value1, ..., valueN */) {
              saveResult(memory, 'rejected', index, slice.apply(arguments));
              check(memory, deferred);
            }
        );
      };

      Deferred.chain =  function () {
        var memory = { resolved: [], rejected: [], count: { total: 0, resolved: 0, rejected: 0 } },
            deferred = new Deferred(),
            promise = deferred.promise();

        promise.join = function() {
          if (arguments.length == 0) deferred.resolve();
          for (var i = 0; i < arguments.length; i++) {
            join(memory, deferred, arguments[i]);
          }
          return promise;
        };
        return promise;        
      };
    })();

    Deferred.when = function() {
      var chain = Deferred.chain(); 
      return chain.join.apply(chain, arguments);
    };
    return Deferred;
  })();

  if (typeof exports !== 'undefined') {
    // Export it as CommonJS module
    module.exports = Deferred;
  }
  else {
    // Expose to the global object
    global.Deferred = Deferred;    
  }
}(this));