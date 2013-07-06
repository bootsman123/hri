filename = report
$(filename).pdf: $(filename).tex
	pdflatex  $(filename)
	bibtex $(filename)
	pdflatex $(filename)
	pdflatex $(filename)

test:
	open $(filename).pdf

clean:
	rm *.bbl *.dvi *.aux *.log *.blg *.pdf

edit:
	vim $(filename).tex

