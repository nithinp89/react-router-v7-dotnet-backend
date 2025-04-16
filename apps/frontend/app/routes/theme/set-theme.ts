import { createThemeAction } from "remix-themes"

import { themeSessionResolver } from "~/utils/theme-sessions.server"

export const action = createThemeAction(themeSessionResolver)
