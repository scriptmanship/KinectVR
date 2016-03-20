<pre>
 ____                                     __
/\  _`\                                  /\ \__       __
\ \ \L\ \     __   _____      __     __  \ \ ,_\     /\_\    ____
 \ \ ,  /   /'__`\/\ '__`\  /'__`\ /'__`\ \ \ \/     \/\ \  /',__\
  \ \ \\ \ /\  __/\ \ \L\ \/\  __//\ \L\.\_\ \ \_  __ \ \ \/\__, `\
   \ \_\ \_\ \____\\ \ ,__/\ \____\ \__/.\_\\ \__\/\_\_\ \ \/\____/
    \/_/\/ /\/____/ \ \ \/  \/____/\/__/\/_/ \/__/\/_/\ \_\ \/___/
                     \ \_\                           \ \____/
                      \/_/                            \/___/

Timed actions in JavaScript simplified
</pre>

[![Build Status](https://secure.travis-ci.org/bjoerge/repeat.js.png)](http://travis-ci.org/bjoerge/repeat.js)

# Summary
Repeat.js is a tiny (< 2kb minified and gzipped) javascript library for timing function calls.

- It abstracts away the need for dealing with setTimeout and setInterval, and the somewhat error prone timer and interval IDs

- It provides an simple, intuitive and flexible chained api.

- It is ideal for polling, performing asynchronous updates, doing timed UI updates or executing expensive tasks asynchronously .

- It can be used standalone or with jQuery.

- It is built on top of a Deferred/promise implemetation for even greater flexibility

- It is tested in IE 7-9, Firefox 10, Chrome 16, Safari 5.1 and Opera 11.61.

## Examples

### Basic example
```javascript
function sayHello() {
  console.log("Hello world!");
};

Repeat(sayHello).every(500, 'ms').for(2, 'minutes').start.in(5, 'sec');
// -> Now wait for 5 seconds and keep a watchful eye on the javascript console
```

### Live display of relative time with jQuery and moment.js

```javascript
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
```

### Asynchronous polling with jQuery 

Polling a server every second, but wait for the request to complete before proceeding

```javascript

Repeat(function(done) {
         jQuery.ajax({
           url: 'http://www.example.com',
           success: function(data) {console.log(data);},
           complete: function() {
             done(); // will wait for this to be called before continuing
           }
         });
       })
       .every(1000, 'ms')
       .for(2, 'minutes')
       .start.in(3, 'secs');
```

### Monitoring changes in an objects' state with console.monitor

Add a function to the console object, that allows for monitoring a property of any object.

```javascript
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
```

### Execute expensive operations asynchronously
A common trick in order to keep the UI responsive while executing expensive operations
is to split up the parts of the operation into smaller subtasks that can be executed asynchronously using timers.

The illustrating example from Nicholas C. Zakas' High Performance Javascript (loc. 3215) could be rewritten like this:

```javascript
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
```

# Promise support
The functions `start()` `now()` `in()` and `wait()` will all return a read-only [Promise](http://en.wikipedia.org/wiki/Futures_and_promises) instance.

- If used with jQuery, it will return an instance of jQuery's [Promise](http://api.jquery.com/Types/#Promise).
- If used standalone, a [minimalistic](https://github.com/bjoerge/dfrrd.js) implementation of the CommonJS [Promises/A](http://wiki.commonjs.org/wiki/Promises/A) spec will be used.

Basic example:

```javascript
Repeat(function() { console.log("W00t"); })
  .every(1, 's')
  .for(20, 's')
  .start.now()
  .then(function() {
    console.log("I'm done w00t'n");
  });
```

## Error handling and promise callbacks

- Any errors thrown in the task function will cause the promise to be rejected with the error object as value
- The promise's progress listeners be notified after each task invocation with the returned value as parameter
- When done, the promise will be resolved with an array of the tasks return value for each invocation

The following example illustrates use of success/progress/error listeners:

```javascript
var onSuccess = function(results) {
  console.log("All good", results);
};
var onFailure = function(exception) {
  console.error("Error", exception);
};
var onProgress = function(result) {
  console.log("Progress: ", result);
};
Repeat(function() {
        var v = Math.random();
        if (v > 0.8) throw new Error("Ouch!");
        return v;
    })
  .every(1, 's')
  .for(10, 's')
  .start()
  .then(onSuccess, onFailure, onProgress);
```

If used standalone or with jQuery, the promise object also provides more convenient `progress` and `fail` functions for adding callbacks in separate steps.
Additionaly, there's also an `always` method to add callback functions for whenever the promise is either resolved or rejected.

The above example using the jQuery or standalone version could be written like this:

```javascript
var repeat = Repeat(function() {
        var v = Math.random();
        if (v > 0.8) throw new Error("Ouch!");
        return v;
    })
  .every(1, 's')
  .for(5, 's')
  .start();

repeat.then(function() { console.log('Resolved!'); });
repeat.fail(function() { console.log('Rejected!'); });
repeat.always(function() { console.log('Done! (either resolved or rejected)'); });
```

## Compatibility notes

- If you are targeting older browsers, you probably want to use any of the ECMAScript 5 polyfill libraries
listed here: https://github.com/Modernizr/Modernizr/wiki/HTML5-Cross-Browser-Polyfills (in particular
 [ddr-ecma5](http://code.google.com/p/ddr-ecma5/), [augmentjs](http://augmentjs.com/) or
  [es5-shim](https://github.com/kriskowal/es5-shim/))

- Some older browsers and environments (IE 6 - 8) doesn't support using reserved words
as property names (and may throw an error when referencing functions like "for", "while" and "in"),
Repeat.js offers alternative function names for code targeted for such environments:

  - `for`   => `lasting`
  - `while` => `during`
  - `in`    => `wait`
  - `if`    => `provided`

Example:

```javascript
Repeat(function() {
  console.log("I'm safe");
 })
 .every(2, 'sec')
 .lasting(2, 'min')                                   // lasting instead of for
 .during(function() { return Math.random() > .5; })   // during instead of while
 .provided(function() { return Math.random() > .5; }) // provided instead of if
 .wait(5, 'sec');                                     // wait instead of in
```

Another less readable (and not recommended) workaround would be to reference these methods using bracket notation:

```javascript
Repeat(function() { console.log("Hello"); })
.every(2, 's')
['for'](2, 'm')
['if'](function() { return Math.random() > .5; })
['while'](function() {  return Math.random() > .5; })
['in'](5, 's');
```

# License
Copyright (C) 2012 Bjørge Næss

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.