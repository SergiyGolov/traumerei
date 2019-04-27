all: clean scss slides

slides:
	pandoc -t revealjs -s -o slides.html slides.md  -V revealjs-url=https://revealjs.com -V css=slides.css --include-after-body=configure.html

scss:
	sass ./sass/app.scss ./slides.css

clean:
	del slides.html, *.css, *.map, *.pdf

export: clean scss slides pdf

pdf:
	decktape -s 2339x1654 slides.html slides.pdf
