"use client"

import Autocomplete from "@mui/material/Autocomplete"
import TextField from "@mui/material/TextField"
import type { SxProps, Theme } from "@mui/material/styles"
import { useState } from "react"

export type ComboboxOption<T extends string | number = string | number> = {
	value: T
	label: string
}

export type ComboboxProps<T extends string | number = string | number> = {
	label?: string
	options: ComboboxOption<T>[]
	defaultValue?: ComboboxOption<T>
	onChange?: (value: ComboboxOption<T> | null) => void
	sx?: SxProps<Theme>
	placeholder?: string
}

const Combobox = <T extends string | number>({
	label = "",
	options,
	defaultValue,
	onChange,
	sx,
	placeholder = "",
}: ComboboxProps<T>) => {
	const [value, setValue] = useState<ComboboxOption<T> | null>(
		defaultValue ?? null,
	)

	return (
		<Autocomplete
			value={value}
			onChange={(_, newValue) => {
				setValue(newValue)
				onChange?.(newValue)
			}}
			options={options}
			size="medium"
			autoComplete
			sx={{ width: 200, ...sx }}
			renderInput={(params) => (
				<TextField
					variant="outlined"
					sx={{
						backgroundColor: "secondary.light",
					}}
					label={label}
					placeholder={placeholder}
					{...params}
				/>
			)}
		/>
	)
}

export default Combobox
