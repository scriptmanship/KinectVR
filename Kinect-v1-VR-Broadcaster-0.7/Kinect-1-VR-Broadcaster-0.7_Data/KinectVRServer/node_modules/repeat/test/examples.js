/**
 * TODO: Include all examples from README
 * 
 * Note: these are not assertions, just examples that should not fail
 */

var Repeat = require('../');

var jQuery = require("jquery");
var moment = require("moment");

var console = {log: function() {}}; // shut it up locally

function expensive() {}
var openDocument = expensive,
    writeText = expensive,
    closeDocument = expensive,
    updateUI = expensive;

describe("Examples (TODO)", function() {
  it("Basic example", function() {
    function sayHello() {
      console.log("Hello world!");
    }

    Repeat(sayHello).every(100, 'ms').for(0.5, 'second').start.in(200);
  });
  it("Live display of relative time with jQuery and moment.js", function() {
    
    jQuery(function() {
      (function mockDOM() { // This is just for creating some example DOM elements
        var ul = jQuery("<ul></ul>"), now = (new Date().getTime()), elem_count = 100;
        while (elem_count--) {;
          ul.append(jQuery('<li class="live-time" data-timestamp="'+
             (now+(Math.random()*100000)*(Math.random() > 0.5 ? -1 : 1)) +
            '"></li>'))
        }
        ul.appendTo(jQuery("body")); 
      }());
    
      var elems = jQuery(".live-time");
      Repeat(function() {
        elems.each(function(i, elem) {
          elem = jQuery(elem);
          var time = moment(elem.data("timestamp"));
          elem.html('Displaying '+ time.format("dddd, MMMM Do YYYY, h:mm:ss a") +' as "<b>'+ time.fromNow()+'</b>"');
        });
      }).every(1000, 'ms').start.now();
    });
  });

  it("Asynchronous polling with jQuery", function() {
    Repeat(function(done) {
       jQuery.ajax({
         url: 'http://www.example.com',
         success: function(data) { console.log(data); },
         complete: function() {
           done(); // will wait for this to be called before continuing
         }
       });
     })
     .every(1, 'sec')
     .for(2, 'minutes')
     .start.in(3, 'secs');
  });
  it("Monitoring changes in an objects' state with console.monitor", function() {
    console.monitor = function(object, property) {
      var last_value = object[property];
      return Repeat(function() {
        var current_value = object[property];
        if (last_value !== current_value) {
          console.log('Property changed from "'+last_value+'" to "'+current_value+'"!');
          last_value = current_value;
        }
        else {
          console.log("No change");
        }
      });
    };
    
    var myObject = {someProperty: "Something"};
    console.monitor(myObject, 'someProperty').every(500, 'ms').for(10, 'seconds').start();
    setTimeout(function() {myObject.someProperty = "Changed value"}, 3000);
  });
  it("Execute expensive operations asynchronously", function() {
    function saveDocument(id) {
      var tasks = [openDocument, writeText, closeDocument, updateUI]; // these are the expensive functions
      return Repeat(function() {
          var task = tasks.shift();
          task(id);
        })
        .async().until(function() {
          return tasks.length == 0;
        }).start.now();        
    }
    saveDocument(1);
  });

  describe("Promise support", function() {
    it("Basic example", function() {
      Repeat(function() { console.log("W00t"); })
        .every(1, 's')
        .for(20, 's')
        .start.now()
        .then(function() {
          console.log("I'm done w00t'n");
        });
    });
    it("Error handling and promise callbacks", function() {
      var onSuccess = function(results) {
        console.log("All good", results);
      };
      var onFailure = function(exception) {
        console.log("Error", exception);
      };
      var onProgress = function(result) {
        console.log("Progress: ", result);
      };

      var v = 0;
      Repeat(function() {
          if (v++ > 2) throw new Error("Ouch!");
          return v;
        })
        .every(1, 's')
        .for(10, 's')
        .start()
        .then(onSuccess, onFailure, onProgress);
    });

    it("jQuery Deferred/Promise support", function() {
      var v = 0;
      var repeat = Repeat(function() {
          if (v++ > 2) throw new Error("Ouch!");
          return v;
        })
        .every(1, 's')
        .for(5, 's')
        .start();
      
      repeat.then(function() { console.log('Resolved!'); });
      repeat.fail(function() { console.log('Rejected!'); });
      repeat.always(function() { console.log('Done! (either resolved or rejected)'); });
    });    
  });

  describe("Compatibility", function() {
    it("Alternative syntax", function() {
      Repeat(function() {
        console.log("I'm safe");
       })
       .every(2, 'sec')
       .lasting(2, 'min')
       .during(function() {return true})
       .provided(function() { return Math.random() > .5; })
       .wait(5, 'sec');
    });
    it("Less readable workaround (not recommended)", function() {
      Repeat(function() { console.log("Hello"); })
      .every(2, 's')
      ['for'](2, 'm')
      ['if'](function() { return Math.random() > .5; })
      ['while'](function() { return true; })
      ['in'](3, 's');
    });
  })
});