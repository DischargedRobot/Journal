"use client"

import MuiCheckbox, { CheckboxProps as MuiCheckboxProps } from "@mui/material/Checkbox"
import CheckboxCheckedIcon from "./CheckboxCheckedIcon"
import CheckboxUncheckedIcon from "./CheckboxUncheckedIcon"

export type CheckboxProps = MuiCheckboxProps

const iconSx = {
	padding: 0,
	width: 32,
	height: 32,
} as const

const Checkbox = ({ sx, ...props }: CheckboxProps) => (
	<MuiCheckbox
		{...props}
		icon={<CheckboxUncheckedIcon />}
		checkedIcon={<CheckboxCheckedIcon />}
		sx={[
			iconSx,
			...(Array.isArray(sx) ? sx : [sx]),
		]}
	/>
)

export default Checkbox
