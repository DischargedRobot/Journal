"use client"

import React, { JSX, useEffect, useState } from "react"
import IconButton, { IconButtonProps } from "@mui/material/IconButton"
import Tooltip from "@mui/material/Tooltip"
import LightModeIcon from "@mui/icons-material/LightMode"
import DarkModeIcon from "@mui/icons-material/DarkMode"
// import { IconProps } from "@mui/material/Icon"

type ThemeMode = "dark" | "light"

type Props = {
	initialTheme?: ThemeMode
	onChange?: (theme: ThemeMode) => void
	maxIconSize?: number
	minIconSize?: number
	color?: string
} & Omit<IconButtonProps, "children">

const ThemeIcon = ({
	initialTheme = "light",
	onChange,
	maxIconSize = 64,
	minIconSize = 24,
	...iconButtonProps
}: Props): JSX.Element => {
	const [theme, setTheme] = useState<ThemeMode>(initialTheme)

	useEffect(() => {
		if (onChange) onChange(theme)
	}, [theme, onChange])

	const toggle = () => setTheme((t) => (t === "light" ? "dark" : "light"))

	return (
		<Tooltip
			title={
				theme === "light"
					? "Переключить на тёмную тему"
					: "Переключить на светлую тему"
			}
		>
			<IconButton
				onClick={toggle}
				aria-label="Сменить тему"
				sx={{ height: 64 }}
				{...iconButtonProps}
			>
				{theme === "light" ? (
					<DarkModeIcon
						sx={{
							color: "black.main",
							fontSize: `clamp(${minIconSize}px, 16.00px + 2.50vw, ${maxIconSize}px)`,
						}}
					/>
				) : (
					<LightModeIcon
						sx={{
							color: "white",
							fontSize: `clamp(${minIconSize}px, 16.00px + 2.50vw, ${maxIconSize}px)`,
						}}
					/>
				)}
			</IconButton>
		</Tooltip>
	)
}

export default ThemeIcon
