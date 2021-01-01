make:

deploy-all: \
deploy-unity-errors\
deploy-unity-json\
deploy-unity-grpc\
deploy-unity-toolkit\
deploy-unity-service-core\
deploy-unity-service-provider-unity

.PHONY: deploy-unity-errors
deploy-unity-errors:
	git subtree push --prefix Assets/com.nathanchambers.unity-errors origin unity-errors

.PHONY: deploy-unity-json
deploy-unity-json:
	git subtree push --prefix Assets/com.nathanchambers.unity-json origin unity-json

.PHONY: deploy-unity-grpc
deploy-unity-grpc:
	git subtree push --prefix Assets/com.nathanchambers.unity-grpc origin unity-grpc

.PHONY: deploy-unity-toolkit
deploy-unity-toolkit:
	git subtree push --prefix Assets/com.nathanchambers.unity-toolkit origin unity-toolkit

.PHONY: deploy-unity-service-core
deploy-unity-service-core:
	git subtree push --prefix Assets/com.nathanchambers.unity-service-core origin unity-service-core

.PHONY: deploy-unity-service-provider-unity
deploy-unity-service-provider-unity:
	git subtree push --prefix Assets/com.nathanchambers.unity-service-provider-unity origin unity-service-provider-unity