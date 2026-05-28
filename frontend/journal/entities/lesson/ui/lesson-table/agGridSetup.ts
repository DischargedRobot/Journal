import { AllCommunityModule, ModuleRegistry } from "ag-grid-community"

let isRegistered = false

/** Регистрация модулей AG Grid (один раз за сессию). */
export const ensureAgGridModules = () => {
	if (isRegistered) {
		return
	}

	ModuleRegistry.registerModules([AllCommunityModule])
	isRegistered = true
}

ensureAgGridModules()
