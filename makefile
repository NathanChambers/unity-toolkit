default:
	@echo

deploy-all: \
deploy-unity-errors\
deploy-unity-json\
deploy-unity-toolkit\
deploy-unity-service-core

.PHONY: deploy-unity-errors
deploy-unity-errors:
	git subtree push --prefix Assets/com.nathanchambers.unity-errors origin unity-errors

.PHONY: deploy-unity-json
deploy-unity-json:
	git subtree push --prefix Assets/com.nathanchambers.unity-json origin unity-json

.PHONY: deploy-unity-toolkit
deploy-unity-toolkit:
	git subtree push --prefix Assets/com.nathanchambers.unity-toolkit origin unity-toolkit

.PHONY: deploy-unity-service-core
deploy-unity-service-core:
	git subtree push --prefix Assets/com.nathanchambers.unity-service-core origin unity-service-core