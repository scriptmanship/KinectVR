SRC = lib/dfrrd.js
DST = dist

REPORTER = dot

all: dfrrd.js dfrrd.min.js

test:
	@NODE_ENV=test ./node_modules/.bin/mocha \
		--compilers coffee:coffee-script \
		--reporter $(REPORTER)

test-cov: lib-cov
	-MOCHA_COVERAGE=1 $(MAKE) test REPORTER=html-cov > coverage.html
	@rm -Rf lib-cov

lib-cov:
	jscoverage lib lib-cov

dfrrd.js: $(SRC)
	@cat $^ > $(DST)/$@
	@node -e "console.log('%sKB %s', (Math.round(require('fs').statSync('$(DST)/$@').size/1024)), '$(DST)/$@')"

dfrrd.min.js: dfrrd.js
	@uglifyjs --no-mangle $(DST)/$< > $(DST)/$@
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
	-@rm -f dist/dfrrd{,.min}.js

.PHONY: test-cov test docs test-docs clean