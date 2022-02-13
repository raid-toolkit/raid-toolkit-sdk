@echo off
rmdir dist /s/q
py -m build
py -m twine upload dist/*