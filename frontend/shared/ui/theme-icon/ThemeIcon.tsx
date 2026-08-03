"use client"

import React, { JSX, useEffect, useState } from "react"
import IconButton, { IconButtonProps } from "@mui/material/IconButton"
import Tooltip from "@mui/material/Tooltip"
import LightModeIcon from "@mui/icons-material/LightMode"
import DarkModeIcon from "@mui/icons-material/DarkMode"
import { fluidClamp } from "@/shared/lib/fluidClampPx"

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
	const iconSize = fluidClamp(minIconSize, maxIconSize)

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
				{...iconButtonProps}
			>
				{theme === "light" ? (
					<DarkModeIcon
						sx={{
							color: "black.main",
							fontSize: iconSize,
						}}
					/>
				) : (
					<LightModeIcon
						sx={{
							color: "white",
							fontSize: iconSize,
						}}
					/>
				)}
			</IconButton>
		</Tooltip>
	)
}

export default ThemeIcon
