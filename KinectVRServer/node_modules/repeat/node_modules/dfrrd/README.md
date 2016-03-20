# Dfrrd

- A ultra-simple and robust Deferred + Promises/A implementation for Node and the browser.

## Project goals

- Keep it simple to the bone
- Keep it closely aligned to the jQuery Deferred API
- Test coverage > 97%

# Download

- <a href="https://raw.github.com/bjoerge/deferred.js/master/dist/dfrrd.js">Development</a>
- <a href="https://raw.github.com/bjoerge/deferred.js/master/dist/dfrrd.min.js">Minified</a>

# How to use

## Include it as a dependency in your project's package.json:

```json
"dependencies": {
  "dfrrd": "latest"
}
```

## Install using npm

    $ npm install dfrrd

# Usage examples

## Node.js

    $ node
    > var Deferred = require("dfrrd")
    > var deferred = new Deferred()
    > deferred.then(function(value) { console.log("Resolved with: ", value) })
    > setTimeout(function() { deferred.resolve("Yay!") }, 1000)
    // ... wait a sec
    > Resolved with: Yay!

## Browser

Copy dfrrd.js and include in your project

    <script src="/path/to/deferred.js"></script>

    <script>
      var deferred = new Deferred()
      deferred.then(function(value) { console.log("Resolved with: ", value) })
      setTimeout(function() { deferred.resolve("Yay!") }, 1000)
    </script>

## Run Mocha tests

  Run tests in Node.js

    $ npm test
    
  Run tests in browser

    $ npm run-script test-browser

## More examples
Check out `test/deferred.coffee` for more examples