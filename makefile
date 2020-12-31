default:
	@echo

deploy-all: deploy-unity-json deploy-unity-toolkit

.PHONY: deploy-unity-json
deploy-unity-json:
	git subtree push --prefix Assets/com.nathanchambers.unity-json origin unity-json

.PHONY: deploy-unity-toolkit
deploy-unity-toolkit:
	git subtree push --prefix Assets/com.nathanchambers.unity-toolkit origin unity-toolkit