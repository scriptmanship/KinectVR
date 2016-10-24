# Remove from cache
delete require.cache[require.resolve("./repeat")]

describe "Repeat works with jQuery", ->
  require("../jquery")
  require("./repeat")
