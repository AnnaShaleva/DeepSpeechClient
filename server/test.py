import pyfreeling
import sys, os
import analyzer

tk, sp, sid, mf, tg, sen = analyzer.set_up_analyzer()
text = "Hello, my beautiful world!"
analyzer.analyze(text, tk, sp, sid, mf, tg, sen)

analyzer.close_session(sp, sid)

