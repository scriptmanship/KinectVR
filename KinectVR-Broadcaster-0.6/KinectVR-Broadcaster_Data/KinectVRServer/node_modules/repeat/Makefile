SRC = lib/repeat.js
DST = dist

REPORTER = dot

all: repeat.js repeat.min.js

test:
	@NODE_ENV=test ./node_modules/.bin/mocha \
		--reporter $(REPORTER) 2> /dev/null

test-cov: lib-cov
	-MOCHA_COVERAGE=1 $(MAKE) test REPORTER=html-cov 1> coverage.html
	@rm -Rf lib-cov

lib-cov:
	jscoverage lib lib-cov

repeat.js: $(SRC)
	@cat $^ > $(DST)/$@
	@node -e "console.log('%sKB %s', (Math.round(require('fs').statSync('$(DST)/$@').size/1024)), '$(DST)/$@')"

repeat.min.js: repeat.js
	@node_modules/.bin/uglifyjs --no-mangle $(DST)/$< > $(DST)/$@
	@node -e "console.log('%sKB %s', (Math.round(require('fs').statSync('$(DST)/$@').size/1024)), '$(DST)/$@')"

test-browser:
	-@node_modules/.bin/coffee test/server test/browser.html

docs: test-docs

test-docs:
	make test REPORTER=doc \
		| cat docs/head.html - docs/tail.html \
		> docs/test.html

clean:
	-@rm -f coverage.html
	-@rm -f dist/repeat{,.min}.js
	-@rm -Rf lib-cov

.PHONY: test-cov test docs test-docs clean