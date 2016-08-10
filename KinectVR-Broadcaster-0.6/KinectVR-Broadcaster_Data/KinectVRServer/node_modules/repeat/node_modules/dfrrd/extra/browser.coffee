require.define "sinon", (require, module, exports) ->
  module.exports = window.sinon

require.define "../deferred", (require, module, exports) ->
  module.exports = window.Deferred