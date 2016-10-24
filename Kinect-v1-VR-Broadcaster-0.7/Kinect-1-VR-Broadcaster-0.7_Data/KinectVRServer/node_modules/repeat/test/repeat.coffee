if this.window?
  Repeat = window.Repeat
  expect = window.expect
else
  Repeat = if process.env.MOCHA_COVERAGE then require('../lib-cov/repeat') else require('../lib/repeat')
  expect = require("expect.js")
  Deferred = require("dfrrd")
  Repeat.defer || (Repeat.defer = -> new Deferred())

## For better readability
after = (delay, func) ->
  setTimeout func, delay

describe "Repeat.js", ->

  describe "Detects common pitfalls and throws an error accordingly", ->
    it "Throws an error if no task is given or task is not a function", ->
      expect(Repeat().start).to.throwException "Don't know any task to run"

    it "Throws an error if trying to start a repetition that has already been started.", ->
      repeat = new Repeat(->).twice()
      repeat.start()  
      expect(repeat.start).to.throwException "Already started"

  describe "Can run both synchronous and asynchronous", ->
    it "runs the task synchronously if no delay is specified (with a call to every() or async())", ->
      counter = 0
      repeat = new Repeat(->counter++)
      repeat.six.times().now()
      expect(counter).to.eql 6

    it "can explicitly be told to run asynchronously by calling async()", (done) ->
      counter = 0
      repeat = new Repeat(->counter++)
      promise = repeat.async().three.times().now()

      expect(counter).to.eql 0

      promise.then -> 
        expect(counter).to.eql 3
        done()

    it "runs all tasks synchronously if no delay is specified", ->
      counter = 0
      repeat = new Repeat(->counter++)
      repeat.six.times().now()
      expect(counter).to.eql 6

  describe "If stop condition is specified by calling while() or until()", ->
    it "Will stop when the function passed to to while() returns false", ->
      counter = 0
      repeat = new Repeat(->counter++)
      proceed = true
      repeat.while -> proceed
      repeat.next()
      repeat.next()
      proceed = false
      repeat.next()
      expect(counter).to.eql 2

    it "Will stop when function passed to until() returns true", (done) ->
      counter = 0
      repeat = new Repeat(->counter++)
      stop = false
      repeat.every(50, "ms").until(->stop).start()

      after 110, -> stop = true

      after 160, ->
        expect(counter).to.eql 3
        done()

    it "Will execute only when function passed to if() returns true", ->
      counter = 0
      repeat = new Repeat(->counter++)
      execute = true
      repeat.if -> execute
      repeat.next()
      expect(counter).to.eql 1
      execute = false
      repeat.next()
      expect(counter).to.eql 1
      execute = true
      repeat.next()
      expect(counter).to.eql 2

    it "Will execute only when function passed to unless() returns false", ->
      counter = 0
      repeat = new Repeat(->counter++)
      skip = false
      repeat.unless -> skip
      repeat.next()
      expect(counter).to.eql 1
      skip = true
      repeat.next()
      expect(counter).to.eql 1
      skip = false
      repeat.next()
      expect(counter).to.eql 2

  describe "API", ->
    describe "task()", ->
      it "can be set by passing it as constructor parameter", ->
        counter = 0
        Repeat(->counter++).once().now()
        expect(counter).to.eql 1

      it "can be defined using the task function", ->
        counter = 0
        Repeat().task(->counter++).once().now()
        expect(counter).to.eql 1

      it "Calling task function will replace any previous registred tasks", ->
        counter = 0
        Repeat(->).task(->).task(->counter++).once().now()
        expect(counter).to.eql 1

    describe "every()", ->
      it "will replace previously registered intervals", (done)->
        counter = 0
        repeat = Repeat(->counter++).every(1, "ms").every(1, "second")
        repeat.start()
        after 100, ->
          expect(counter).to.eql 1
          done()

      it "can be replaced at any time througout the lifetime of the repetition to change the frequency", (done)->
        counter = 0
        repeat = Repeat(-> counter++).every(50, 'ms')
        repeat.start()

        after 65, -> repeat.every 100, "ms" # <-- change frequency after 40 ms

        after 10, -> expect(counter).to.eql 1
        after 55, -> expect(counter).to.eql 2
        after 100, -> expect(counter).to.eql 2
        after 160, ->expect(counter).to.eql 3

        after 170, ->
          repeat.cancel().then ->
            done()

  describe "Provides an API to run tasks at any given time", ->
    it "allows for manually executing task the given number of times by calling next()", ->
      counter = 0
      repeat = new Repeat(->counter++)
      repeat.next()
      repeat.next()
      expect(counter).to.eql 2

    it "will stop when task has been called the number of times defined by calling times()", ->
      counter = 0
      repeat = new Repeat(->counter++)
      repeat.three.times()
      repeat.next()
      repeat.next()
      repeat.next()
      repeat.next()
      repeat.next()
      expect(counter).to.eql 3

  describe "for", ->
    it "will not execute task after number of milliseconds specified by for() has passed", (done) ->

      counter = 0
      repeat = new Repeat(->counter++).for(30, "ms")

      after 10, -> repeat.next()
      after 20, -> repeat.next()
      after 50, -> repeat.next()
      after 50, -> repeat.next()

      after 70, ->
        expect(counter).to.eql 2
        repeat.stop().then -> done()

  describe "It stops immediately if stop is invoked before started", ->
    it "Will wait for asynchronous task to finish before proceeding", (test_done) ->
      
      counter = waiting: 0, finished: 0
      repeat = new Repeat (done) ->
        counter.waiting++
        after 40, ->
          counter.finished++
          done()

      repeat.every(60, "ms").now()

      after 30, ->
        expect(counter.waiting).to.eql 1   # first task is started, but...
        expect(counter.finished).to.eql 0  # ... not finished yet

      after 50, ->
        expect(counter.waiting).to.eql 1    # first task started...
        expect(counter.finished).to.eql 1   # ..and finished

      after 110, ->
        expect(counter.waiting).to.eql 2    # second task started...
        expect(counter.finished).to.eql 1   # ..but not finished yet

      after 150, ->
        expect(counter.waiting).to.eql 2    # first task started...
        expect(counter.finished).to.eql 2   # ..and finished      

        repeat.cancel().then -> test_done()
