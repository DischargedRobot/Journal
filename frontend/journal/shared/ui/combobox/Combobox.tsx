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
	label: string
	options: ComboboxOption<T>[]
	defaultValue?: ComboboxOption<T>
	onChange?: (value: ComboboxOption<T> | null) => void
	sx?: SxProps<Theme>
}

const Combobox = <T extends string | number>({
	label,
	options,
	defaultValue,
	onChange,
	sx,
}: ComboboxProps<T>) => {
	const [value, setValue] = useState<ComboboxOption<T>>(
		defaultValue ?? options[0] ?? null,
	)

	return (
		<Autocomplete
			value={value}
			onChange={(_, newValue) => {
				setValue(newValue)
				onChange?.(newValue)
			}}
			options={options}
			disableClearable
			size="medium"
			autoComplete
			sx={{ width: 200, ...sx }}
			renderInput={(params) => (
				<TextField
					{...params}
					label={label}

				/>
			)}
		/>
	)
}

export default Combobox
